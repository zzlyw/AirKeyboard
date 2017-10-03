using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class stereoview : MonoBehaviour
{



    float[,] K_matrix;
    float[,] R;
    float[,] T;

    public GameObject CameraTest;
    void Awake()
    {

     
        K_matrix = new float[3, 3];
        R = new float[3, 3];
        T = new float[3, 1];

     
        ReadRT("LeapRT.txt", R,T);



        Matrix4x4 M = Matrix4x4.zero;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                M[i, j] = (float)R[i, j];
            }
        }

        M.SetColumn(3, new Vector4((float)T[0, 0], (float)T[1, 0], (float)T[2, 0], 1));
        M.SetRow(3, new Vector4(0, 0, 0, 1));
        TransformFromMatrix(AxisFromM(M), CameraTest.transform);

       //  CameraTest.transform.Rotate(Vector3.right * 180, Space.Self);

       // CameraTest.transform.localPosition += testLeap.correct;



        
      
      

    }


    void Update()
    {
        

      
       
      
    }

    void CreateFile(string path, string name, string info)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();
        }

        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();

    }
    void ReadK(string fileName, float[,] K)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(fileName);
        }
        catch (Exception e)
        {
            return;
        }
        string line;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if ((line = sr.ReadLine()) != null)
                {
                    K[i, j] = float.Parse(line);
                }
            }
        }

        sr.Close();
        sr.Dispose();

    }

    void ReadRT(string fileName, float[,] R, float[,]  T)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(fileName);
        }
        catch (Exception e)
        {
            return;
        }
        string line;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if ((line = sr.ReadLine()) != null)
                {
                    R[i, j] = float.Parse(line);
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 1; j++)
            {
                if ((line = sr.ReadLine()) != null)
                {
                    T[i, j] = float.Parse(line);
                }
            }
        }

        sr.Close();
        sr.Dispose();

    }
    Matrix4x4 AxisFromM(Matrix4x4 m)
    {
        Matrix4x4 result = Matrix4x4.zero;

        Vector4 rx0 = m.GetRow(0);
        Vector4 rx1 = m.GetRow(1);
        Vector4 rx2 = m.GetRow(2);

        Vector4 tx = m.GetColumn(3);
        //将R转置
        result.SetColumn(0, rx0);
        result.SetColumn(1, rx1);
        result.SetColumn(2, rx2);

        tx.w = 0;

        Vector4 r0 = result.GetRow(0);
        Vector4 r1 = result.GetRow(1);
        Vector4 r2 = result.GetRow(2);

        Vector4 T = new Vector4(Vector4.Dot(r0, tx), Vector4.Dot(r1, tx), Vector4.Dot(r2, tx), 1);
        result.SetColumn(3, -T);
        result.SetRow(3, new Vector4(0, 0, 0, 1));
        //Debug.Log(result);
        return result;
        //return m;
    }


    private void TransformFromMatrix(Matrix4x4 matrix, Transform trans)
    {
        trans.localRotation = QuaternionFromMatrix(matrix);

        trans.localPosition = matrix.GetColumn(3); // uses implicit conversion from Vector4 to Vector3
    }



    private Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm

        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;

        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));

        return q;

    }
}
