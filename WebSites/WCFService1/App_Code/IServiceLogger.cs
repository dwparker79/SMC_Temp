using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

/// <summary>
/// Summary description for IServiceLogger
/// </summary>
[ServiceContract]
public interface IServiceLogger
{
    [OperationContract]
    List<List<string>> GetLogs(string[] serviceNames, string machineName, int pageNum, int logsPerPage);

    [OperationContract]
    List<List<string>> GetLogPage(int pageNum, int logsPerPage, string[] serviceNameFilters);

    [OperationContract]
    int GetNumPages(int logsPerPage, string[] filter);
    
    [OperationContract]
    void WriteLog(string log);
}