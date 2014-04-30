namespace nex.Utilities.Serialization
{
    public interface ISerializationHelperUser
    {
        /// <summary>
        /// Key of class for serialization
        /// </summary>
        string SerializationKey { get; }

        //OPT: In .net 4.0 change to Tuple
        /// <summary>
        /// Get data to serialize from classs
        /// </summary>
        /// <returns>Data to serialize</returns>
        SerializationData GetDataToSave();

        /// <summary>
        /// Get data from SerializationHelper and apply it to class
        /// </summary>
        void ApplyLoadedData();
    }
}