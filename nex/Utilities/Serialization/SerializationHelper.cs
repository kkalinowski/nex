using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace nex.Utilities.Serialization
{
    /// <summary>
    /// Class using to serialize static classes
    /// </summary>
    [Serializable]
    public static class SerializationHelper
    {
        private static List<SerializationData> data;
        private static List<ISerializationHelperUser> users;

        public static bool DataLoaded { get; private set; }

        /// <summary>
        /// File, to which save data
        /// </summary>
        public const string DataPath = "nex.dat";

        static SerializationHelper()
        {
            DataLoaded = false;
            data = new List<SerializationData>();
            users = new List<ISerializationHelperUser>();
        }

        /// <summary>
        /// Save data in file
        /// </summary>
        public static void Save()
        {
            data.Clear();

            SerializationData val;
            foreach (ISerializationHelperUser user in users)
            {
                val = user.GetDataToSave();
                data.Add(val);
            }

            try
            {
                FileStream fs = new FileStream(DataPath, FileMode.Create, FileAccess.Write, FileShare.None);

                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, data);
                }
                catch
                {
                    MessageBox.Show("Nie mogę zapisać danych.");
                }
                finally
                {
                    fs.Close();
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Nie mogę uzyskać dostępu do pliku z danymi. Brak uprawnień"); 
            }
            catch (Exception)
            {
                MessageBox.Show("Nie mogę zapisać danych.");
            }
        }

        /// <summary>
        /// Load data from file
        /// </summary>
        public static void Load()
        {
            try
            {
                FileStream fs = new FileStream(DataPath, FileMode.Open, FileAccess.Read, FileShare.None);

                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (List<SerializationData>)bf.Deserialize(fs);
                    DataLoaded = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot load current settings. Reason - " + ex);
                }
                finally
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open file to load settings. Reason - " + ex);
            }
        }

        public static SerializationData GetData(string key)
        {
            return (from d in data
                    where d.Key == key
                    select d).SingleOrDefault();
        }

        public static void RegisterUser(ISerializationHelperUser user)
        {
            users.Add(user);
        }
    }
}