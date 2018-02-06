using Assets.MolecularMachinesAdapter.Adapters;
using Assets.MolecularMachinesAdapter.Import;
using Assets.MolecularMachinesAdapter.Utils;
using MolecularMachines.Framework.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MolecularMachinesAdapter
{
    public class MMM : MonoBehaviour
    {
        private static MMM _instance = null;
        public static MMM Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<MMM>();
                if (_instance == null)
                {
                    var go = GameObject.Find("_MMM");
                    if (go != null) DestroyImmediate(go);

                    go = new GameObject("_MMM");
                    _instance = go.AddComponent<MMM>();
                }

                return _instance;
            }
        }

        public void Poke()
        { }

        public static MMEnvironmentUnity Environment
        {
            get { return MMM.Instance.environment; }
        }

        public bool AllowEditFixed = false;

        public bool showInfo = true;
        public bool collidersActive = true;

        private MMEnvironmentUnity environment;
        private JsonImport import;
        private string path;
        private List<EnvironmentSource> environmentSources = new List<EnvironmentSource>();

        private UnityEngine.UI.Text textInfo;
        private InfoTextLog infoText = InfoTextLog.Instance; // new InfoText();

        public bool waitForUserInput = false;
        public bool userInput = false;
        public static bool WaitForUserInput()
        {
            if (MMM.Instance.waitForUserInput)
            {
                if (MMM.Instance.userInput)
                {
                    MMM.Instance.userInput = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        private bool environmentLoaded = false;
        public void Start()
        {
            var config = EnvironmentConfig.Find(typeof(MMM));
            path = config.XmlGetPath("display");
            Debug.Log("Display files from path: " + path);

            this.environment = new MMEnvironmentUnity();

            this.import = new JsonImport(this.environment);

            this.textInfo = GameObject.Find("TextInfo").GetComponent<UnityEngine.UI.Text>();

            MolecularMachines.Framework.Logging.Log.Writer = new LogWriterUnity();

            ImportEnvironmentSources();
        }

        private void ImportEnvironmentSources()
        {
            this.environmentSources.Clear();
            this.environmentSources.AddRange(
                Directory.GetDirectories(this.path)
                    .Select(folderPath => new EnvironmentSource(folderPath, this.import))
            );
        }

        public void Update()
        {
            InfoTextLog.Instance.Reset();
            environment.Update();

            textInfo.text = showInfo ? infoText.GetText() : "";
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                environment.DrawGizmos();
            }
        }

        public float ccWidth = 200;
        public float ccHeight = 62;
        public float ccTop = 36;
        public float ccLeft = 10;
        public float ccSpacing = 8;
        public float ccBoxTop = 28;
        public float ccButtonSize = 30;

        private bool ccEnabled = true;

        void OnGUI()
        {
            float y = ccTop;

            GUI.backgroundColor = UnityEngine.Color.white;
            GUI.color = UnityEngine.Color.white;

            if (GUI.Button(new Rect(ccLeft, y, ccButtonSize, ccButtonSize), ccEnabled ? "\u25b2" : "\u25bc"))
            {
                ccEnabled = !ccEnabled;
            }

            y += ccButtonSize + ccSpacing;

            if (ccEnabled)
            {
                foreach (var cc in this.environment.ConcentrationControllers)
                {
                    GUI.Box(new Rect(ccLeft, y, ccWidth, ccHeight - ccSpacing), cc.Name + "(" + cc.CurrentCount + ")");
                    var setCount = (int)GUI.HorizontalSlider(new Rect(ccLeft + ccSpacing, y + ccBoxTop, ccWidth - 2 * ccSpacing, ccHeight), cc.CurrentCount, cc.MinCount, cc.MaxCount);
                    if (setCount != cc.CurrentCount)
                    {
                        cc.SetCount(setCount);
                    }

                    y += ccHeight;
                }

                y += ccSpacing;

                if (!environmentLoaded)
                {
                    foreach (var item in this.environmentSources)
                    {
                        if (GUI.Button(new Rect(ccLeft, y, ccWidth, ccButtonSize), item.Name))
                        {
                            item.Load();
                            environmentLoaded = true;
                        }

                        y += ccButtonSize + ccSpacing;
                    }
                }
            }
        }
    }
}
