﻿using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Fomm.Util;

namespace Fomm.Games.Fallout3.Tools.AutoSorter
{
  /// <summary>
  ///   Utility class that handles checking for, and retreiving, new
  ///   version of the load order template.
  /// </summary>
  public class Fallout3BOSSUpdater
  {
    private static Regex m_rgxVersion = new Regex(@"Revision (\d+): ");

    /// <summary>
    ///   Gets the URL where the latest masterlist lives.
    /// </summary>
    /// <value>The URL where the latest masterlist lives.</value>
    protected virtual string MasterListURL
    {
      get
      {
        return Properties.Settings.Default.fallout3MasterListUpdateUrl;
      }
    }

    /// <summary>
    ///   Gets the current verison of the BOSS Fallout 3 Masterlist.
    /// </summary>
    /// <returns>The current verison of the BOSS Fallout 3 Masterlist.</returns>
    public Int32 GetMasterlistVersion()
    {
      string strVersionPage;
      using (var wclGetter = new WebClient())
      {
        var strMasterListUrl = MasterListURL;
        var intLastDividerPos = strMasterListUrl.LastIndexOfAny(new[]
        {
          Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar
        });
        var strVersionUrl = strMasterListUrl.Substring(0, intLastDividerPos);
        strVersionPage = wclGetter.DownloadString(strVersionUrl);
      }

      var strWebVersion = m_rgxVersion.Match(strVersionPage).Groups[1].Value.Trim();
      return Int32.Parse(strWebVersion);
    }

    /// <summary>
    ///   Updates the BOSS Fallout 3 Masterlist used by FOMM.
    /// </summary>
    public void UpdateMasterlist(string p_strPath)
    {
      string strMasterlist;
      using (var wclGetter = new WebClient())
      {
        //the substring is to remove the 3byte EFBBBF Byte Order Mark (BOM)
        strMasterlist = TextUtil.ByteToString(wclGetter.DownloadData(MasterListURL));
      }
      File.WriteAllText(p_strPath, GetMasterlistVersion() + Environment.NewLine + strMasterlist);
    }
  }
}