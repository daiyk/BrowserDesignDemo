using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrowserDesign.users;


    /// <summary>
    /// This represent a user
    /// Add default setting to here
    /// </summary>
    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public string Language { get; set; } = "EN";
        public int defaultCrsUser { get; set; } = 3857;
        public int defaultAltitudeModelUser { get; set; } = 1; // as the enum AltitudeOffsets in CommonAltitudeOffsets
        public bool isUsingPin { get; set; }
        public string EsriName { get; set; }
        public string EsriDomain { get; set; }
        public string Encryption { get; set; }
        public string EncryptionVec { get; set; }

        //oauth2 relevent data
        public string refreshToken { get; set; }
        public bool isOAuth2 { get; set; }
        public string Oauth2ExpiresAt { get; set; }

        //app login encryption
        public string LoginEncryption { get; set; }
        public string LoginEncryptionVec { get; set; }


        public string ServiceDirectoryURL { get; set; }

        public List<UserItem> userItems { get; set; }
    }

    [Serializable]
    public class Profile
    {
        public List<User> SavedUsers { get; set; }
        public string LastSaved { get; set; }

    }

