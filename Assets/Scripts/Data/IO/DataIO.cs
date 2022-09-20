using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Http;
using BrowserDesign.UI;

#if UNITY_WSA && ENABLE_WINMD_SUPPORT
    using Windows.Storage;
#endif

/// <summary>
/// Helper class to handle IO operations
/// </summary>
public class DataIO
{
   
    public async static Task<bool> IsFileExistInSourceFolder(string filePath)
    {
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
        StorageFolder sourceDir = DataFolders.sourceDirIO;

        // this shows only the default save path
        // this is not gonna work!
        Debug.Log($"sourceDir {sourceDir.Path}");

        // get relative path
        //Debug.Log("----------------------------- dirPath: " + dirPath + "-----------------------------------------");
        StorageFolder selectedFolder;
        string relPath = GetRelativePath(sourceDir.Path, filePath);
        Debug.Log($"relPath {relPath}");
        if(relPath.Equals(filePath)){
            Debug.LogError($"Error: Acess file from non-standard folder: {filePath}");
            return false;
        }

        if(await sourceDir.TryGetItemAsync(relPath)!=null)
        {
            return true;
        }
        else
        {
            return false;
        }
#else
        if (System.IO.File.Exists(filePath))
        {
            return true;
        }
        return false;
#endif
    }


    #region PathUtils
    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">Contains the directory that defines the start of the relative path.(lib)</param>
    /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from the start directory to the end path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetRelativePath(string fromPath, string toPath)
    {

        // check if topath is a filename
        if (Path.GetDirectoryName(toPath) == string.Empty || Path.GetDirectoryName(toPath) == "")
        {
            return "";
        }


        if (string.IsNullOrEmpty(fromPath))
        {
            return toPath;
        }

        if (string.IsNullOrEmpty(toPath))
        {
            throw new ArgumentNullException("toPath");
        }

        Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
        Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

        if (fromUri.Scheme != toUri.Scheme)
        {
            return toPath;
        }

        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        return relativePath;
    }

    private static string AppendDirectorySeparatorChar(string path)
    {
        // Append a slash only if the path is a directory and does not have a slash.
        if (!Path.HasExtension(path) &&
            !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            return path + Path.DirectorySeparatorChar;
        }

        return path;
    }

    #endregion

    #region storagefolders
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
    // Gets a storagefolder from a path (normally not possible under UWP)
    public static async Task<StorageFolder> GetStorageFolderFromFullPath(string fullPath)
    {
        // get relative path from "Documents"
        string relPath = DataIO.GetRelativePath(DataFolders.defaultSaveLibPath, fullPath);

        //
        StorageFolder directory;
        if (relPath != "" && relPath!=Path.GetFileName(fullPath))
        {
            directory = await DataFolders.sourceDirWSA.GetFolderAsync(Path.GetDirectoryName(relPath));
        }
        else
        {
            directory = DataFolders.sourceDirWSA;
        }
        return directory;
    }

    // check if file exists
    public static async Task<bool> DoesFileExists(string fullPath)
    {
        string filename = System.IO.Path.GetFileName(fullPath);

        // get relative path from "Documents"
        string relPath = DataIO.GetRelativePath(DataFolders.defaultSaveLibPath, fullPath);

        // get containing folder
        StorageFolder directory;
        if (relPath != "" && relPath != filename)
        {
            directory = await DataFolders.sourceDirWSA.GetFolderAsync(Path.GetDirectoryName(relPath));
        }
        else
        {
            directory = DataFolders.sourceDirWSA;
        }

        StorageFile file;
        var itExist = await directory.TryGetItemAsync(filename);

        if (itExist != null)
            return true;
        else
            return false;
    }
#endif


    #endregion

    #region JsonIO
    public static async Task<bool> SaveJsonToFile<T>(string filePath, T objectToSave, Encoding encodeWay = null)
    {
        //create file and save to the path
        if(Path.GetExtension(filePath) != ".json")
        {
            Debug.LogError("SaveJsonToFile: file extension is not Json");
            return false;
        }
        //deault we use Unicode encoding
        if(encodeWay == null)
        {
            encodeWay = Encoding.Unicode;
        }
        string jsonString = JsonConvert.SerializeObject(objectToSave);
        //save to file
        try
        {
            using (var stream = File.OpenWrite(filePath))
            {
                //truncated and dispose all the existing content
                stream.SetLength(0);
                var bytes = encodeWay.GetBytes(jsonString);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }

    }
    
    //read file from local folder, need to use UWP StorageFolder
    public static async Task<T> ReadJsonFromFile<T>(string filePath, Encoding encodeWay = null)
    {
        if (Path.GetExtension(filePath) != ".json")
        {
            Debug.LogError("ReadJsonToFile: file extension is not Json");
            return default(T);
        }
        if (encodeWay == null)
        {
            encodeWay = Encoding.Unicode;
        }

#if UNITY_WSA && ENABLE_WINMD_SUPPORT
        Windows.Storage.Streams.UnicodeEncoding unicodeWay;
        if (encodeWay.Equals(Encoding.Unicode))
        {
            unicodeWay = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;
        }
        else if(encodeWay.Equals(Encoding.BigEndianUnicode))
        {
            unicodeWay = Windows.Storage.Streams.UnicodeEncoding.Utf16BE;
        }
        else if (encodeWay.Equals(Encoding.UTF8))
        {
            unicodeWay = Windows.Storage.Streams.UnicodeEncoding.Utf8;
        }
        else
        {
            return default(T);
        }

        try
        {
            var fileLoaded = await StorageFile.GetFileFromPathAsync(filePath);
            string text = await FileIO.ReadTextAsync(fileLoaded,unicodeWay);
            T jsonObject = JsonConvert.DeserializeObject<T>(text);
            return jsonObject;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return default(T);
        }
#else
        try
        {
            T jsonObject;
            using (StreamReader streamReader = new StreamReader(filePath, encodeWay))
            {
                string text = streamReader.ReadToEnd();
                jsonObject = JsonConvert.DeserializeObject<T>(text);
            }
            return jsonObject;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return default(T);
        }
#endif
    }

    #endregion

}
