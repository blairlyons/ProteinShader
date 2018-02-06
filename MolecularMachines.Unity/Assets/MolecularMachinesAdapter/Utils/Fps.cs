using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Fps : MonoBehaviour
{
    class Average
    {
        public Average(int count)
        {
            values = new float[count];
            index = 0;
        }

        private float[] values;
        private int index = 0;

        private float avg = 0;

        public void Push(float x)
        {
            int n = values.Length;
            index = (index + 1) % n;

            var oldValue = values[index];
            values[index] = x;

            avg = avg + (x - oldValue) / n;
        }

        public float Avg
        {
            get { return avg; }
        }
    }

    private Average avg = new Average(20);
    private Text text;

    public void Start()
    {
        text = GetComponent<Text>();
    }

    private DateTime nextUpdate = DateTime.Now;

    public void Update()
    {
        avg.Push(Time.deltaTime);

        if (DateTime.Now > nextUpdate)
        {
            nextUpdate = DateTime.Now.AddSeconds(0.5);

            double fps = double.NaN;
            var avgT = avg.Avg;
            if (avgT > 0)
            {
                fps = 1.0 / avgT;
                text.text = fps.ToString("0.00") + " FPS";
            }
            else
            {
                text.text = "No FPS " + DateTime.Now.Ticks;
            }
        }

    }
}
