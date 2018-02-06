using EnvironmentConfigLib;
using MolecularMachines.Import.Hemoglobin;
using MolecularMachines.Import.Microtubule;
using MolecularMachines.Import.AtpSynthase;
using MolecularMachines.Import.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using MolecularMachines.Framework.Logging;
using MolecularMachines.Import.KinesinConcept;
using MolecularMachines.Import.Kinesin2;

namespace MolecularMachines.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read Folder Paths from config
            EnvironmentConfig config = EnvironmentConfig.Find();
            pdbs = config.XmlGetPath("pdbs");
            display = config.XmlGetPath("display");

            // Clear Display Folder
            ImportUtils.ClearFolder(display);

            // Import
            DoImportHemoglobin();
            DoImportAtpSynthase();
            //DoImportMicrotubule();
            //DoImportKinesinConcept();
            DoImportKinsein2();

            // Copy this Program as DLL to Unity Project
            // so it can use the EntityBehaviors and EntityFactories defined in this assembly
            Log.Info("copy import assembly to Unity Assets");
            var codeBaseUri = Assembly.GetExecutingAssembly().CodeBase;
            var importExePath = new Uri(codeBaseUri).LocalPath;
            var unityAssetsPath = config.XmlGetPath("unityAssets");
            File.Copy(importExePath, Path.Combine(unityAssetsPath, Path.GetFileNameWithoutExtension(importExePath) + ".dll"), true);

            Console.WriteLine("done.");
        }

        private static string pdbs;
        private static string display;

        private static void DoImportAtpSynthase()
        {
            var import = new ImportAtpSynthase();
            import.Import(Path.Combine(pdbs, "atp_synthase"));
            import.CreateTestEnvironment();
            import.Save(display, "ATP-Synthase");
        }

        private static void DoImportHemoglobin()
        {
            var import = new ImportHemoglobin();
            import.Import(Path.Combine(pdbs, "hemoglobin"));
            //import.CreateEntityOverview();
            import.CreateTestEnvironment();
            //import.CreateSingleHemoglobin();
            import.Save(display, "Hemoglobin");
        }

        private static void DoImportMicrotubule()
        {
            var import = new ImportMicrotubule();
            import.Import(Path.Combine(pdbs, "microtubule"));
            import.Save(display, "Microtubule");
        }

        private static void DoImportKinesinConcept()
        {
            var import = new ImportKinesinConcept();
            import.Import(Path.Combine(Path.Combine(pdbs, "kinesin"), "model1"));
            import.Save(display, "Kinesin (Concept)");
        }

        private static void DoImportKinsein2()
        {
            var import = new ImportKinesin2();
            import.Import(Path.Combine(Path.Combine(pdbs, "kinesin"), "model2"));
            import.CreateTestEnvironment();
            import.Save(display, "Kinesin");
        }
    }
}
