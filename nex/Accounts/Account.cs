using System;

namespace nex.Accounts
{
    [Serializable]
    public class Account
    {
        #region Props
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public bool IsDefault { get; set; } 
        #endregion

        #region ctor
        public Account(string url, string userName, string password, bool isDefault)
        {
            Url = url;
            UserName = userName;
            Password = password;
            IsDefault = isDefault;
        } 
        #endregion

        #region ToString
        public override string ToString()
        {
            return string.Concat(UserName, " @ ", Url, IsDefault ? " - Default" : "");
        } 
        #endregion
    }
}