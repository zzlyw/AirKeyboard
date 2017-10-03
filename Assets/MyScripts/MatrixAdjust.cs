using UnityEngine;
using System.Collections;
using System.IO;

[ExecuteInEditMode]
public class MatrixAdjust : MonoBehaviour
{
    public Vector3 m0;
    public Vector3 m1;
    public Vector3 m2;
    public float[] KParameters;

    //定义XY调整变量
    public static float ScreenXCenter = 0;
    public static float ScreenYCenter = 0;

    public static bool isRevise = false;
    public static bool startAutoCorrection = false;
    void Awake()
    {
        KParameters = new float[9];
        System.IO.StreamReader readK = new System.IO.StreamReader("R_KParameters.txt");
        float[] dataK = new float[9];
        string strTemp;
        for (int i = 0; i < 9; i++)
        {

            strTemp = readK.ReadLine();
            dataK[i] = float.Parse(strTemp);
            KParameters[i] = dataK[i];
           

        }
        readK.Close();


        m0.x = KParameters[0];
        m0.y = KParameters[1];
        m0.z = KParameters[2];
        //m1.x = KParameters[3];
        m1.y = KParameters[4];
        m1.z = KParameters[5];
       // m2.x = KParameters[6];
       // m2.y = KParameters[7];
       // m2.z = KParameters[8];

        ScreenXCenter = m0.z;
        ScreenYCenter = m1.z;


        m0.z = ScreenXCenter;
        m1.z = ScreenYCenter;
        Camera cam = GetComponent<Camera>();
        Matrix4x4 m = MatrixFromK(m0, m1, m2);
        cam.projectionMatrix = m;


    }
    void Update()
    {
        if (startAutoCorrection && isRevise)
        {

            //调整渲染平面
            m0.z = ScreenXCenter - 0.5f*ReviseError.screenError.x;
            m1.z = ScreenYCenter - 0.5f*ReviseError.screenError.y;
            Camera cam = GetComponent<Camera>();
            Matrix4x4 m = MatrixFromK(m0, m1, m2);
            cam.projectionMatrix = m;

            //调整虚拟摄像机位置
            float xc = ReviseError.screenError.x / 6400;
            float yc = ReviseError.screenError.y / 6400;
            this.transform.Translate(Vector3.right * xc, Space.Self);
            this.transform.Translate(Vector3.up * yc, Space.Self);

            //复位
            isRevise = false;
        }
        
    }
    //void LateUpdate()
    //{
    //    m0.z = ScreenXCenter;
    //    m1.z = ScreenYCenter;
    //    Camera cam = GetComponent<Camera>();
    //    Matrix4x4 m = MatrixFromK(m0, m1, m2);
       
    //    cam.projectionMatrix = m;
        
    //}
    public  void Opt()
    {
        m0.z = ScreenXCenter;
        m1.z = ScreenYCenter;
        Camera cam = GetComponent<Camera>();
        Matrix4x4 m = MatrixFromK(m0, m1, m2);

        cam.projectionMatrix = m;
    }
    static Matrix4x4 MatrixFromK(Vector3 m0,Vector3 m1,Vector3 m2)
    {
        Matrix4x4 m = new Matrix4x4();
        float k00 = m0.x;
        float k01 = m0.y;
        float k02 = m0.z;

        float width = m1.x;
        float k11 = m1.y;
        float k12 = m1.z;

        float height = m2.x;
        float znear = m2.y;
        float zfar = m2.z;

        m[0, 0] = 2 * k00 / width;
        m[0, 1] = - 2 * k01 / width;
        m[0, 2] = (width - 2 * k02) / width;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = 2 * k11 / height;
        m[1, 2] = (height - 2 * k12) / height;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = (-zfar - znear) / (zfar - znear);
        m[2, 3] = -2 * zfar * znear / (zfar - znear);
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = -1;
        m[3, 3] = 0;


        return m;
    }
}
