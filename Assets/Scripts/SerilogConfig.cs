using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;

public class SerilogConfig : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        // ���� Serilog ����
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Unity3D()
            .CreateLogger();
    }

    void OnDestroy()
    {
        // �ر� Serilog ��־��¼��
        Log.CloseAndFlush();
    }
}
