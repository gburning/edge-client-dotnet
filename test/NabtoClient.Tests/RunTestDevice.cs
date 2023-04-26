using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Nabto.Edge.Client.Tests;



public class DeviceConfig {
    public string ProductId { get; set; }
    public string DeviceId { get; set; }
}

public class TestDeviceRunner : IDisposable {

    private string _iamConfig = @"    {
   ""Config"" : {
      ""UnpairedRole"" : ""Unpaired""
   },
   ""Policies"" : [
      {
         ""Id"" : ""Pairing"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""IAM:GetPairing"",
                  ""IAM:PairingPasswordInvite"",
                  ""IAM:PairingLocalInitial"",
                  ""IAM:PairingLocalOpen"",
                  ""IAM:PairingPasswordOpen""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      },
      {
         ""Id"" : ""Tunnelling"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""TcpTunnel:GetService"",
                  ""TcpTunnel:Connect"",
                  ""TcpTunnel:ListServices""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      },
      {
         ""Id"" : ""ManageIAM"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""IAM:ListUsers"",
                  ""IAM:GetUser"",
                  ""IAM:DeleteUser"",
                  ""IAM:SetUserRole"",
                  ""IAM:ListRoles"",
                  ""IAM:CreateUser"",
                  ""IAM:SetUserPassword"",
                  ""IAM:SetUserDisplayName"",
                  ""IAM:SetUserFingerprint"",
                  ""IAM:SetUserSct"",
                  ""IAM:SetUserUsername"",
                  ""IAM:SetUserFcm"",
                  ""IAM:SetSettings"",
                  ""IAM:GetSettings"",
                  ""IAM:SetDeviceInfo"",
                  ""IAM:SetUserNotificationCategories"",
                  ""IAM:ListNotificationCategories""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      },
      {
         ""Id"" : ""ManageOwnUser"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""IAM:GetUser"",
                  ""IAM:DeleteUser"",
                  ""IAM:SetUserDisplayName""
               ],
               ""Conditions"" : [
                  {
                     ""StringEquals"" : {
                        ""IAM:Username"" : [
                           ""${Connection:Username}""
                        ]
                     }
                  }
               ],
               ""Effect"" : ""Allow""
            },
            {
               ""Actions"" : [
                  ""IAM:ListRoles"",
                  ""IAM:ListUsers""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      }
   ],
   ""Roles"" : [
      {
         ""Id"" : ""Unpaired"",
         ""Policies"" : [
            ""Pairing"",
            ""ManageIAM""
         ]
      },
      {
         ""Id"" : ""Administrator"",
         ""Policies"" : [
            ""ManageIAM"",
            ""Tunnelling"",
            ""Pairing""
         ]
      },
      {
         ""Id"" : ""Administrator2"",
         ""Policies"" : [
            ""ManageIAM"",
            ""Tunnelling"",
            ""Pairing""
         ]
      },
      {
         ""Id"" : ""Standard"",
         ""Policies"" : [
            ""Tunnelling"",
            ""Pairing"",
            ""ManageOwnUser""
         ]
      },
      {
         ""Id"" : ""Guest"",
         ""Policies"" : [
            ""Pairing"",
            ""ManageOwnUser""
         ]
      }
   ],
   ""Version"" : 1
}";

    private string _tempPath;
    private string _logsDir;


    private StreamWriter _standardError;
    private StreamWriter _standardOutput;
    private Guid _uuid;

    private Process _deviceProcess;

    public string ProductId { get; set; } = TestUtil.RandomProductId();
    public string DeviceId { get; set; } = TestUtil.RandomDeviceId();

    public TestDeviceRunner()
    {
        _uuid = Guid.NewGuid();
        _tempPath = Path.GetTempPath() + "device-" + _uuid;
        Directory.CreateDirectory(_tempPath);
        init();
        StartDevice();
    }

    public void Dispose()
    {
      Console.WriteLine("########### DESTRUCTOR ###########");
        StopDevice();
    }

    ~TestDeviceRunner() {
    }

    public void init() {
        string configDir = Path.Combine(_tempPath, "config");
        string keysDir = Path.Combine(_tempPath, "keys");
        string stateDir = Path.Combine(_tempPath, "state");
        _logsDir = Path.Combine(_tempPath, "logs");
        Directory.CreateDirectory(configDir);
        Directory.CreateDirectory(keysDir);
        Directory.CreateDirectory(stateDir);
        Directory.CreateDirectory(_logsDir);

        WriteConfig(configDir);
        WriteIamConfig(configDir);
        WriteServicesConfig(configDir);
        WriteKey(keysDir);
        WriteState(stateDir);

      _standardError = new StreamWriter(Path.Combine(_logsDir, "error.log"));
      _standardOutput = new StreamWriter(Path.Combine(_logsDir, "output.log"));

    }

    public void WriteConfig(string configDir) {
        var deviceConfig = new DeviceConfig { ProductId = ProductId, DeviceId = DeviceId };
        string jsonString = JsonSerializer.Serialize(deviceConfig);
        var sw = new StreamWriter(Path.Combine(configDir, "device.json"));
        sw.Write(jsonString);
        sw.Close();
    }

    public void WriteIamConfig(string configDir)
    {
        var sw = new StreamWriter(Path.Combine(configDir, "tcp_tunnel_device_iam_config.json"));
        sw.Write(_iamConfig);
        sw.Close();
    }

    public void WriteServicesConfig(string configDir)
    {
        var sw = new StreamWriter(Path.Combine(configDir, "tcp_tunnel_device_services.json"));
        sw.Write("[]");
        sw.Close();
    }

    public void WriteKey(string keysDir) {
        var client = NabtoClient.Create();
        string privateKey = client.CreatePrivateKey();
        var sw = new StreamWriter(Path.Combine(keysDir, "device.key"));
        sw.Write(privateKey);
        sw.Close();
    }

    public void WriteState(string stateDir)
    {
        string defaultState = @"{""Version"":1,""OpenPairingPassword"":""rHfMdaw4zpne"",""OpenPairingSct"":""zAsxwgETYzrX"",""LocalOpenPairing"":true,""PasswordOpenPairing"":true,""PasswordInvitePairing"":true,""LocalInitialPairing"":true,""OpenPairingRole"":""Administrator"",""InitialPairingUsername"":""admin"",""FriendlyName"":""ZiCS1UxVBTcEP6Db"", ""Users"": {""Username"": ""admin"", ""role"": ""Administrator""}}";
        var sw = new StreamWriter(Path.Combine(stateDir, "tcp_tunnel_device_iam_state.json"));

        sw.Write(defaultState);
        sw.Close();
    }


    public void StartDevice() {
         ProcessStartInfo info = new ProcessStartInfo();
         info.FileName = "/workspaces/edge-client-dotnet/test-devices/tcp_tunnel_device_linux";
         info.Arguments = $"-H {_tempPath} --random-ports";
        info.RedirectStandardError = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;

        _deviceProcess = new Process
        {
            StartInfo = info
        };
        _deviceProcess.Start();

       _ = ReadOutputAsync(_deviceProcess.StandardError, _standardError);
       _ = ReadOutputAsync(_deviceProcess.StandardOutput, _standardOutput);
    }

    public async Task ReadOutputAsync(StreamReader sr, StreamWriter sw) {

        try
        {
            string? line;
            do
            {
                line = await sr.ReadLineAsync();
                if (line != null)
                {
                    await sw.WriteLineAsync(line);
                }
            } while (line != null);

        } catch (Exception e) {
            Console.WriteLine(e);
        }
    }

    public void StopDevice() {
        _deviceProcess.Kill();
        _standardError.Close();
        _standardOutput.Close();
    }


}
