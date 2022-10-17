using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BossForce : MonoBehaviour
{
    public float wx = -1.0f; // weight max
    public float wn; // weight min
    public float x0 = -3.0f;
    public float x1 = -1.0f;
    public float x2 = 1.0f;
    public float x3 = 3.0f;
    public float y0 = -1.5f;
    public float y1 = 1.5f;

    public float xn;
    public float xx;
    public float yn;
    public float yx;

    public float dx;
    public float dy;

    private int[] idxs = new int[]{
        0,1,2,3, 0,1,3,2, 0,2,1,3, 0,2,3,1, 0,3,1,2, 0,3,2,1,
        1,0,2,3, 1,0,3,2, 1,2,0,3, 1,2,3,0, 1,3,0,2, 1,3,2,0,
        2,0,1,3, 2,0,3,1, 2,1,0,3, 2,1,3,0, 2,3,0,1, 2,3,1,0,
        3,0,1,2, 3,0,2,1, 3,1,0,2, 3,1,2,0, 3,2,0,1, 3,2,1,0
    };

    private Queue<ForceResult> filecode;

    void Start()
    {
        CheckArrange();
        filecode = new Queue<ForceResult>();
    }

    public bool checker;

    void Update()
    {
        if(checker)
        {
            checker = false;
            FileOutAll();
            /*
            int width = 10;
            int height = 10;
            int a = 255;

            var builder = PngBuilder.Create(width, height, false);

            Pixel px1 = new Pixel(127, 65, 94);
            Pixel px2 = new Pixel(3, 9, 87);
            Pixel px3 = new Pixel(96, 198, 76);

            builder.SetPixel(px1, 0, 0);
            builder.SetPixel(px2, 9, 9);
            builder.SetPixel(px3, 3, 7);

            string imgpath = "C:/Programming/myImg.png";

            using(FileStream fs = new FileStream(imgpath, FileMode.Create))
            {
                builder.Save(fs);
            }

            var img = Png.Open(imgpath);*/
        }

        while(filecode.Count > 0)
            filecode.Dequeue().Print();
    }

    private void Swap(ref float a, ref float b)
    {
        if(a > b)
        {
            float t = a;
            a = b;
            b = t;
        }
    }

    private void CheckArrange()
    {
        Swap(ref x0, ref x1);
        Swap(ref x0, ref x2);
        Swap(ref x0, ref x3);
        Swap(ref x1, ref x2);
        Swap(ref x1, ref x3);
        Swap(ref x2, ref x3);
        Swap(ref y0, ref y1);
    }

    private void FileOutAll()
    {
        int lptimes = 24;
        float[] arr = new float[4];

        arr[0] = x0;
        arr[1] = x1;
        arr[2] = x2;
        arr[3] = x3;

        ForceData data = new ForceData();
        string nameForm = "bossForceArea_{0:d2}";

        data.y0 = y0;
        data.y1 = y1;
        data.wn = wn;
        data.wx = wx;
        data.xn = xn;
        data.xx = xx;
        data.yn = yn;
        data.yx = yx;
        data.dx = dx;
        data.dy = dy;

        for(int i = 0; i < lptimes; i++)
        {
            int ptr = 4 * i;

            int i0 = idxs[ptr];
            int i1 = idxs[ptr + 1];
            int i2 = idxs[ptr + 2];
            int i3 = idxs[ptr + 3];

            data.x0 = arr[i0];
            data.x1 = arr[i1];
            data.x2 = arr[i2];
            data.x3 = arr[i3];

            Action action = () =>
            {
                // FileOut(data, string.Format(nameForm, i + 1));
            };

            Task tk = new Task(action);
            tk.Start();
            tk.Wait();
        }
    }

    private void FileOut(ForceData data, string filename)
    {
        StringBuilder message = data.GetMessage();
        string path = string.Format("C:/Programming/{0}.txt", filename);

        try
        {
            using(FileStream fs = new FileStream(path, FileMode.Create))
            using(StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(message.ToString());
            }

            filecode.Enqueue(new ForceResult(true, path));
        }
        catch(Exception)
        {
            filecode.Enqueue(new ForceResult(false, path));
        }
    }
}

public struct ForceResult
{
    private bool success;
    private string path;

    public ForceResult(bool s, string p)
    {
        success = s;
        path = p;
    }

    public void Print()
    {
        if(success)
            Debug.Log(string.Format("Completed save bossForce file. (at {0})", path));
        else
            Debug.Log(string.Format("Cannot save bossForce file. (at {0})", path));
    }
}

public struct ForceData
{
    // NOTE: x0 < x1 < x2 < x3
    public float x0;
    public float x1;
    public float x2;
    public float x3;

    // NOTE: y0 < y1
    public float y0;
    public float y1;

    public float wn;
    public float wx;

    // NOTE: xn < xx
    public float xn;
    public float xx;

    // NOTE: yn < yx
    public float yn;
    public float yx;

    public float dx;
    public float dy;

    public float GetValue(float px, float py)
    {
        if(py < y0 || py > y1 || px < x0 || px > x3)
            return wn;
        else if(px < x1)
            return (wx - wn) * (px - x0) / (x1 - x0);
        else if(px > x2)
            return (wx - wn) * (x3 - px) / (x3 - x2);
        else
            return wx;
    }

    public StringBuilder GetMessage()
    {
        StringBuilder msg = new StringBuilder();

        float i, j;
        float val;

        for(i = yx; i >= yn; i -= dy)
        {
            for(j = xn; j <= xx; j += dx)
            {
                val = GetValue(j, i);

                if(val < 0.0f)
                    msg.AppendFormat("{0:0.0000} ", val);
                else
                    msg.AppendFormat("{0:0.00000} ", val);
            }
            msg.Append("\n");
        }

        return msg;
    }
}

/*
    string imgpath = "C:/Programming/myImg.png";
    int width = 10;
    int height = 10;

    var builder = PngBuilder.Create(width, height, false);

    Pixel px1 = new Pixel(127, 65, 94);

    builder.SetPixel(px1, 0, 0);

    using(FileStream fs = new FileStream(imgpath, FileMode.Create))
    {
        builder.Save(fs);
    }
*/