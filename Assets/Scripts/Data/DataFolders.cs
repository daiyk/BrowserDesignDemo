using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;

#if UNITY_WSA && ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.FileProperties;
#endif


/// <summary>
/// Keeps track of source, temp and write directories
/// </summary>
public class DataFolders : MonoBehaviour
{
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
    /// <summary>
    /// Where temporary files are stored
    /// </summary>
    public static StorageFolder tempDirWSA = ApplicationData.Current.LocalFolder;

    public static StorageFolder sourceDirIO = ApplicationData.Current.LocalFolder;

    public static StorageFolder tempDirIO = ApplicationData.Current.LocalCacheFolder;

    /// <summary>
    /// Where the files are stored
    /// </summary>
    public static StorageFolder sourceDirWSA = KnownFolders.DocumentsLibrary;

    // default save location of the lib (workaround)
    public static string defaultSaveLibPath;
#endif
#if UNITY_EDITOR
    /// <summary>
    /// Where temporary files are stored, this is redirected to Unity project: <project_folder>/tempState
    /// </summary>
    public static string tempDirIO = UnityEngine.Windows.Directory.temporaryFolder;
    /// <summary>
    /// Where the files are stored
    /// </summary>
    public static string sourceDirIO = Application.persistentDataPath;

#endif

    /// <summary>
    /// Workaround method for working with folders in UWP
    /// </summary>
    public static async void SetSourceDirDefaultPath()
    {
        // We can't get the path of a library (virtual folder) but we still need to specify a path in order to work with the file browser
        // So as a path we get the default save location of the lib
        // It's obviously gonna be different dependinng on the library choosen. So here we solve it, assuming that we're gonna work only with pic and doc lib
        // This is valid only under uwp
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
        string libDefaultPath;
        if (sourceDirWSA.DisplayName == "Pictures")
        {
            var picLib = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            defaultSaveLibPath = picLib.SaveFolder.Path;
        }
        else if (sourceDirWSA.DisplayName == "Documents" || sourceDirWSA.DisplayName == "Documenten" || sourceDirWSA.DisplayName == "Documentos" || sourceDirWSA.DisplayName == "Documenti" || sourceDirWSA.DisplayName == "Dokumente")
        {
            var docLib = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Documents);
            defaultSaveLibPath = docLib.SaveFolder.Path;
        }
#endif
    }

}
