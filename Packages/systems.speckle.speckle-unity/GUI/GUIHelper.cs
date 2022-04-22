﻿using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Api;
using Speckle.Core.Credentials;

namespace Speckle.ConnectorUnity.GUI
{
  public static class GUIHelper
  {

    private const char SEP = '-';
    private const string DEFAULT = "empty";

    #region account
    public static string Format(this Account item)
    {
      return item != null ? item.userInfo.email + SEP + item.serverInfo.name : string.Empty;
    }

    public static string ParseAccountEmail(this string value)
    {
      return value.Valid() ? value.Split(SEP).FirstOrDefault() : null;
    }

    public static string ParseAccountServer(this string value)
    {
      return value.Valid() ? value.Split(SEP).Last() : null;
    }

    public static IEnumerable<string> Format(this IEnumerable<Account> items)
    {
      return items != null ? items.Select(x => x.Format()).ToArray() : new[] { DEFAULT };
    }
    #endregion

    #region Stream
    public static IEnumerable<string> Format(this IEnumerable<Stream> items)
    {
      return items != null ? items.Select(x => x.Format()).ToArray() : new[] { DEFAULT };
    }

    public static string Format(this Stream item)
    {
      return item != null ? item.name + SEP + item.id : string.Empty;
    }

    public static string ParseStreamName(this string value)
    {
      return value.Valid() ? value.Split(SEP).FirstOrDefault() : null;
    }

    public static string ParseStreamId(this string value)
    {
      return value.Valid() ? value.Split(SEP).Last() : null;
    }
    #endregion

    #region Branch
    public static string Format(this Branch item)
    {
      return item != null ? item.name : string.Empty;
    }

    public static IEnumerable<string> Format(this IEnumerable<Branch> items)
    {
      return items != null ? items.Select(x => x.Format()).ToArray() : new[] { DEFAULT };
    }
    #endregion

    #region Commit
    public static IEnumerable<string> Format(this IEnumerable<Commit> items)
    {
      return items != null ? items.Select(x => x.Format()).ToArray() : new[] { DEFAULT };
    }

    public static string Format(this Commit item)
    {
      return item != null ? item.id + SEP + item.message : string.Empty;
    }

    public static string ParseCommitId(this string value)
    {
      return value.Valid() ? value.Split(SEP).FirstOrDefault() : null;
    }

    public static string ParseCommitMsg(this string value)
    {
      return value.Valid() ? value.Split(SEP).Last() : null;
    }
    #endregion

  }
}