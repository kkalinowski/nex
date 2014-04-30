using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nex.Controls;
using System.IO;
using System.Windows.Controls;
using nex.Controls.DirectoryViews;

namespace nexTest
{
    /// <summary>
    /// Summary description for ClassATest
    /// </summary>
    [TestClass]
    public class ClassATest
    {
        public ClassATest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;
        private static string dirLevel0;
        private static string dirLevel1;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            dirLevel0 = Path.GetTempPath() + "\\" + Path.GetRandomFileName();

            Directory.CreateDirectory(dirLevel0);

            for (int i = 0; i < 50; i++)
                File.CreateText(dirLevel0 + "\\" + Path.GetRandomFileName() + ".txt").Close();

            dirLevel1 = dirLevel0 + "\\" + "level1";
            Directory.CreateDirectory(dirLevel1);

            for (int i = 0; i < 10; i++)
                File.CreateText(dirLevel1 + "\\" + Path.GetRandomFileName() + ".txt").Close();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Directory.Delete(dirLevel0, true);
        }

        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        [TestMethod]
        public void A0_ShowDirContentTest()
        {
            DirectoryViewContainer dv = new DirectoryViewContainer();

            string[] infos = Directory.GetFileSystemEntries(dirLevel0);

            dv.LoadDir(dirLevel0,false);

            Assert.AreEqual(infos.Length + 1, dv.Items.Count);
        }

        [TestMethod]
        public void A72_DirectoryHierarchyMovingtTest()
        {
            DirectoryViewContainer dv = new DirectoryViewContainer();

            string[] infos0 = Directory.GetFileSystemEntries(dirLevel0);
            string[] infos1 = Directory.GetFileSystemEntries(dirLevel1);

            dv.LoadDir(dirLevel1,false);
            Assert.AreEqual(infos1.Length + 1, dv.Items.Count);

            dv.MoveUp();
            Assert.AreEqual(infos0.Length + 1, dv.Items.Count);
        }
    }
}
