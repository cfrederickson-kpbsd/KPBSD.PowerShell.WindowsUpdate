using System;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public static class ComErrorCodes
    {
        public static ErrorRecord CreateErrorRecord(int errorCode, Exception? exception, object? targetObject)
        {
            TryGetErrorDetails(errorCode, out var errorId, out var category, out var message);
            if (exception is null)
            {
                exception = new COMException(message, errorCode);
            }
            var er = new ErrorRecord(
                exception,
                errorId,
                category,
                targetObject
            );
            er.ErrorDetails = new ErrorDetails(exception.Message);
            er.ErrorDetails.RecommendedAction = "See the error documentation for details. https://docs.microsoft.com/en-us/windows/deployment/update/windows-update-error-reference";
            return er;
        }
        public static bool TryGetErrorDetails(int errorCode, out string errorId, out ErrorCategory category, out string message)
        {
            switch (unchecked((uint)errorCode))
            {
                case 0x80243FFF:
                    {
                        errorId = "WU_E_AUCLIENT_UNEXPECTED";
                        message = "There was a user interface error not covered by another WU_E_AUCLIENT_* error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024A000:
                    {
                        errorId = "WU_E_AU_NOSERVICE";
                        message = "Automatic Updates was unable to service incoming requests.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x8024A002:
                    {
                        errorId = "WU_E_AU_NONLEGACYSERVER";
                        message = "The old version of the Automatic Updates client has stopped because the WSUS server has been upgraded.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x8024A003:
                    {
                        errorId = "WU_E_AU_LEGACYCLIENTDISABLED";
                        message = "The old version of the Automatic Updates client was disabled.";
                        category = ErrorCategory.NotEnabled;
                        return true;
                    }
                case 0x8024A004:
                    {
                        errorId = "WU_E_AU_PAUSED";
                        message = "Automatic Updates was unable to process incoming requests because it was paused.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x8024A005:
                    {
                        errorId = "WU_E_AU_NO_REGISTERED_SERVICE";
                        message = "No unmanaged service is registered with AU.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x8024AFFF:
                    {
                        errorId = "WU_E_AU_UNEXPECTED";
                        message = "An Automatic Updates error not covered by another WU_E_AU* code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80243001:
                    {
                        errorId = "WU_E_INSTALLATION_RESULTS_UNKNOWN_VERSION";
                        message = "The results of download and installation could not be read from the registry due to an unrecognized data format version.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80243002:
                    {
                        errorId = "WU_E_INSTALLATION_RESULTS_INVALID_DATA";
                        message = "The results of download and installation could not be read from the registry due to an invalid data format.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80243003:
                    {
                        errorId = "WU_E_INSTALLATION_RESULTS_NOT_FOUND";
                        message = "The results of download and installation are not available; the operation may have failed to start.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80243004:
                    {
                        errorId = "WU_E_TRAYICON_FAILURE";
                        message = "A failure occurred when trying to create an icon in the taskbar notification area.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80243FFD:
                    {
                        errorId = "WU_E_NON_UI_MODE";
                        message = "Unable to show UI when in non-UI mode; WU client UI modules may not be installed.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80243FFE:
                    {
                        errorId = "WU_E_WUCLTUI_UNSUPPORTED_VERSION";
                        message = "Unsupported version of WU client UI exported functions.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                // this error code is covered elsewhere
                // case 0x80243FFF:
                //     {
                //         errorId = "WU_E_AUCLIENT_UNEXPECTED";
                //         message = "There was a user interface error not covered by another WU_E_AUCLIENT_* error code.";
                //         category = ErrorCategory.NotSpecified;
                //         return true;
                //     }
                case 0x8024043D:
                    {
                        errorId = "WU_E_SERVICEPROP_NOTAVAIL";
                        message = "The requested service property is not available.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80249001:
                    {
                        errorId = "WU_E_INVENTORY_PARSEFAILED";
                        message = "Parsing of the rule file failed.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80249002:
                    {
                        errorId = "WU_E_INVENTORY_GET_INVENTORY_TYPE_FAILED";
                        message = "Failed to get the requested inventory type from the server.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80249003:
                    {
                        errorId = "WU_E_INVENTORY_RESULT_UPLOAD_FAILED";
                        message = "Failed to upload inventory result to the server.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80249004:
                    {
                        errorId = "WU_E_INVENTORY_UNEXPECTED";
                        message = "There was an inventory error not covered by another error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80249005:
                    {
                        errorId = "WU_E_INVENTORY_WMI_ERROR";
                        message = "A WMI error occurred when enumerating the instances for a particular class.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x8024E001:
                    {
                        errorId = "WU_E_EE_UNKNOWN_EXPRESSION";
                        message = "An expression evaluator operation could not be completed because an expression was unrecognized.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024E002:
                    {
                        errorId = "WU_E_EE_INVALID_EXPRESSION";
                        message = "An expression evaluator operation could not be completed because an expression was invalid.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024E003:
                    {
                        errorId = "WU_E_EE_MISSING_METADATA";
                        message = "An expression evaluator operation could not be completed because an expression contains an incorrect number of metadata nodes.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x8024E004:
                    {
                        errorId = "WU_E_EE_INVALID_VERSION";
                        message = "An expression evaluator operation could not be completed because the version of the serialized expression data is invalid.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x8024E005:
                    {
                        errorId = "WU_E_EE_NOT_INITIALIZED";
                        message = "The expression evaluator could not be initialized.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024E006:
                    {
                        errorId = "WU_E_EE_INVALID_ATTRIBUTEDATA";
                        message = "An expression evaluator operation could not be completed because there was an invalid attribute.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024E007:
                    {
                        errorId = "WU_E_EE_CLUSTER_ERROR";
                        message = "An expression evaluator operation could not be completed because the cluster state of the computer could not be determined.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024EFFF:
                    {
                        errorId = "WU_E_EE_UNEXPECTED";
                        message = "There was an expression evaluator error not covered by another WU_E_EE_* error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80247001:
                    {
                        errorId = "WU_E_OL_INVALID_SCANFILE";
                        message = "An operation could not be completed because the scan package was invalid.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80247002:
                    {
                        errorId = "WU_E_OL_NEWCLIENT_REQUIRED";
                        message = "An operation could not be completed because the scan package requires a greater version of the Windows Update Agent.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80247FFF:
                    {
                        errorId = "WU_E_OL_UNEXPECTED";
                        message = "Search using the scan package failed.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024F001:
                    {
                        errorId = "WU_E_REPORTER_EVENTCACHECORRUPT";
                        message = "The event cache file was defective.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024F002:
                    {
                        errorId = "WU_E_REPORTER_EVENTNAMESPACEPARSEFAILED";
                        message = "The XML in the event namespace descriptor could not be parsed.";
                        category = ErrorCategory.ParserError;
                        return true;
                    }
                case 0x8024F003:
                    {
                        errorId = "WU_E_INVALID_EVENT";
                        message = "The XML in the event namespace descriptor could not be parsed.";
                        category = ErrorCategory.ParserError;
                        return true;
                    }
                case 0x8024F004:
                    {
                        errorId = "WU_E_SERVER_BUSY";
                        message = "The server rejected an event because the server was too busy.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x8024FFFF:
                    {
                        errorId = "WU_E_REPORTER_UNEXPECTED";
                        message = "There was a reporter error not covered by another error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80245001:
                    {
                        errorId = "WU_E_REDIRECTOR_LOAD_XML";
                        message = "The redirector XML document could not be loaded into the DOM class.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80245002:
                    {
                        errorId = "WU_E_REDIRECTOR_S_FALSE";
                        message = "The redirector XML document is missing some required information.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80245003:
                    {
                        errorId = "WU_E_REDIRECTOR_ID_SMALLER";
                        message = "The redirectorId in the downloaded redirector cab is less than in the cached cab.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80245FFF:
                    {
                        errorId = "WU_E_REDIRECTOR_UNEXPECTED";
                        message = "The redirector failed for reasons not covered by another WU_E_REDIRECTOR_* error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244000:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_BASE";
                        message = "WU_E_PT_SOAPCLIENT_* error codes map to the SOAPCLIENT_ERROR enum of the ATL Server Library.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244001:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_INITIALIZE";
                        message = "Same as SOAPCLIENT_INITIALIZE_ERROR - initialization of the SOAP client failed possibly because of an MSXML installation failure.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244002:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_OUTOFMEMORY";
                        message = "Same as SOAPCLIENT_OUTOFMEMORY - SOAP client failed because it ran out of memory.";
                        category = ErrorCategory.LimitsExceeded;
                        return true;
                    }
                case 0x80244003:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_GENERATE";
                        message = "Same as SOAPCLIENT_GENERATE_ERROR - SOAP client failed to generate the request.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244004:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_CONNECT";
                        message = "Same as SOAPCLIENT_CONNECT_ERROR - SOAP client failed to connect to the server.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80244005:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_SEND";
                        message = "Same as SOAPCLIENT_SEND_ERROR - SOAP client failed to send a message for reasons of WU_E_WINHTTP_* error codes.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80244006:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_SERVER";
                        message = "Same as SOAPCLIENT_SERVER_ERROR - SOAP client failed because there was a server error.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244007:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_SOAPFAULT";
                        message = "Same as SOAPCLIENT_SOAPFAULT - SOAP client failed because there was a SOAP fault for reasons of WU_E_PT_SOAP_* error codes.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244008:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_PARSEFAULT";
                        message = "Same as SOAPCLIENT_PARSEFAULT_ERROR - SOAP client failed to parse a SOAP fault.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80244009:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_READ";
                        message = "Same as SOAPCLIENT_READ_ERROR - SOAP client failed while reading the response from the server.";
                        category = ErrorCategory.ReadError;
                        return true;
                    }
                case 0x8024400A:
                    {
                        errorId = "WU_E_PT_SOAPCLIENT_PARSE";
                        message = "Same as SOAPCLIENT_PARSE_ERROR - SOAP client failed to parse the response from the server.";
                        category = ErrorCategory.ReadError;
                        return true;
                    }
                case 0x8024400B:
                    {
                        errorId = "WU_E_PT_SOAP_VERSION";
                        message = "Same as SOAP_E_VERSION_MISMATCH - SOAP client found an unrecognizable namespace for the SOAP envelope.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024400C:
                    {
                        errorId = "WU_E_PT_SOAP_MUST_UNDERSTAND";
                        message = "Same as SOAP_E_MUST_UNDERSTAND - SOAP client was unable to understand a header.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024400D:
                    {
                        errorId = "WU_E_PT_SOAP_CLIENT";
                        message = "Same as SOAP_E_CLIENT - SOAP client found the message was malformed; fix before resending.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024400E:
                    {
                        errorId = "WU_E_PT_SOAP_SERVER";
                        message = "Same as SOAP_E_SERVER - The SOAP message could not be processed due to a server error; resend later.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024400F:
                    {
                        errorId = "WU_E_PT_WMI_ERROR";
                        message = "There was an unspecified Windows Management Instrumentation (WMI) error.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244010:
                    {
                        errorId = "WU_E_PT_EXCEEDED_MAX_SERVER_TRIPS";
                        message = "The number of round trips to the server exceeded the maximum limit.";
                        category = ErrorCategory.QuotaExceeded;
                        return true;
                    }
                case 0x80244011:
                    {
                        errorId = "WU_E_PT_SUS_SERVER_NOT_SET";
                        message = "WUServer policy value is missing in the registry.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80244012:
                    {
                        errorId = "WU_E_PT_DOUBLE_INITIALIZATION";
                        message = "Initialization failed because the object was already initialized.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80244013:
                    {
                        errorId = "WU_E_PT_INVALID_COMPUTER_NAME";
                        message = "The computer name could not be determined.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80244015:
                    {
                        errorId = "WU_E_PT_REFRESH_CACHE_REQUIRED";
                        message = "The reply from the server indicates that the server was changed or the cookie was invalid; refresh the state of the internal cache and retry.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244016:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_BAD_REQUEST";
                        message = "Same as HTTP status 400 - the server could not process the request due to invalid syntax.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80244017:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_DENIED";
                        message = "Same as HTTP status 401 - the requested resource requires user authentication.";
                        category = ErrorCategory.AuthenticationError;
                        return true;
                    }
                case 0x80244018:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_FORBIDDEN";
                        message = "Same as HTTP status 403 - server understood the request but declined to fulfill it.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x80244019:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_NOT_FOUND";
                        message = "Same as HTTP status 404 - the server cannot find the requested URI (Uniform Resource Identifier).";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x8024401A:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_BAD_METHOD";
                        message = "Same as HTTP status 405 - the HTTP method is not allowed.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x8024401B:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_PROXY_AUTH_REQ";
                        message = "Same as HTTP status 407 - proxy authentication is required.";
                        category = ErrorCategory.AuthenticationError;
                        return true;
                    }
                case 0x8024401C:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_REQUEST_TIMEOUT";
                        message = "Same as HTTP status 408 - the server timed out waiting for the request.";
                        category = ErrorCategory.OperationTimeout;
                        return true;
                    }
                case 0x8024401D:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_CONFLICT";
                        message = "Same as HTTP status 409 - the request was not completed due to a conflict with the current state of the resource.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024401E:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_GONE";
                        message = "Same as HTTP status 410 - requested resource is no longer available at the server.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x8024401F:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_SERVER_ERROR";
                        message = "Same as HTTP status 500 - an error internal to the server prevented fulfilling the request.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244020:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_NOT_SUPPORTED";
                        message = "Same as HTTP status 500 - server does not support the functionality required to fulfill the request.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244021:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_BAD_GATEWAY";
                        message = "Same as HTTP status 502 - the server while acting as a gateway or a proxy received an invalid response from the upstream server it accessed in attempting to fulfill the request.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80244022:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_SERVICE_UNAVAIL";
                        message = "Same as HTTP status 503 - the service is temporarily overloaded.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80244023:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_GATEWAY_TIMEOUT";
                        message = "Same as HTTP status 503 - the request was timed out waiting for a gateway.";
                        category = ErrorCategory.OperationTimeout;
                        return true;
                    }
                case 0x80244024:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_VERSION_NOT_SUP";
                        message = "Same as HTTP status 505 - the server does not support the HTTP protocol version used for the request.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80244025:
                    {
                        errorId = "WU_E_PT_FILE_LOCATIONS_CHANGED";
                        message = "Operation failed due to a changed file location; refresh internal state and resend.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80244026:
                    {
                        errorId = "WU_E_PT_REGISTRATION_NOT_SUPPORTED";
                        message = "Operation failed because Windows Update Agent does not support registration with a non-WSUS server.";
                        category = ErrorCategory.InvalidArgument;
                        return true;
                    }
                case 0x80244027:
                    {
                        errorId = "WU_E_PT_NO_AUTH_PLUGINS_REQUESTED";
                        message = "The server returned an empty authentication information list.";
                        category = ErrorCategory.AuthenticationError;
                        return true;
                    }
                case 0x80244028:
                    {
                        errorId = "WU_E_PT_NO_AUTH_COOKIES_CREATED";
                        message = "Windows Update Agent was unable to create any valid authentication cookies.";
                        category = ErrorCategory.AuthenticationError;
                        return true;
                    }
                case 0x80244029:
                    {
                        errorId = "WU_E_PT_INVALID_CONFIG_PROP";
                        message = "A configuration property value was wrong.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024402A:
                    {
                        errorId = "WU_E_PT_CONFIG_PROP_MISSING";
                        message = "A configuration property value was missing.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024402B:
                    {
                        errorId = "WU_E_PT_HTTP_STATUS_NOT_MAPPED";
                        message = "The HTTP request could not be completed and the reason did not correspond to any of the WU_E_PT_HTTP_* error codes.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024402C:
                    {
                        errorId = "WU_E_PT_WINHTTP_NAME_NOT_RESOLVED";
                        message = "Same as ERROR_WINHTTP_NAME_NOT_RESOLVED - the proxy server or target server name cannot be resolved.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x8024402F:
                    {
                        errorId = "WU_E_PT_ECP_SUCCEEDED_WITH_ERRORS";
                        message = "External cab file processing completed with some errors.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244030:
                    {
                        errorId = "WU_E_PT_ECP_INIT_FAILED";
                        message = "The external cab processor initialization did not complete.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80244031:
                    {
                        errorId = "WU_E_PT_ECP_INVALID_FILE_FORMAT";
                        message = "The format of a metadata file was invalid.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80244032:
                    {
                        errorId = "WU_E_PT_ECP_INVALID_METADATA";
                        message = "External cab processor found invalid metadata.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80244033:
                    {
                        errorId = "WU_E_PT_ECP_FAILURE_TO_EXTRACT_DIGEST";
                        message = "The file digest could not be extracted from an external cab file.";
                        category = ErrorCategory.ReadError;
                        return true;
                    }
                case 0x80244034:
                    {
                        errorId = "WU_E_PT_ECP_FAILURE_TO_DECOMPRESS_CAB_FILE";
                        message = "An external cab file could not be decompressed.";
                        category = ErrorCategory.ReadError;
                        return true;
                    }
                case 0x80244035:
                    {
                        errorId = "WU_E_PT_ECP_FILE_LOCATION_ERROR";
                        message = "External cab processor was unable to get file locations.";
                        category = ErrorCategory.ReadError;
                        return true;
                    }
                case 0x80244FFF:
                    {
                        errorId = "WU_E_PT_UNEXPECTED";
                        message = "A communication error not covered by another WU_E_PT_* error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024502D:
                    {
                        errorId = "WU_E_PT_SAME_REDIR_ID";
                        message = "Windows Update Agent failed to download a redirector cabinet file with a new redirectorId value from the server during the recovery.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x8024502E:
                    {
                        errorId = "WU_E_PT_NO_MANAGED_RECOVER";
                        message = "A redirector recovery action did not complete because the server is managed.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x80246001:
                    {
                        errorId = "WU_E_DM_URLNOTAVAILABLE";
                        message = "A download manager operation could not be completed because the requested file does not have a URL.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80246002:
                    {
                        errorId = "WU_E_DM_INCORRECTFILEHASH";
                        message = "A download manager operation could not be completed because the file digest was not recognized.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80246003:
                    {
                        errorId = "WU_E_DM_UNKNOWNALGORITHM";
                        message = "A download manager operation could not be completed because the file metadata requested an unrecognized hash algorithm.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80246004:
                    {
                        errorId = "WU_E_DM_NEEDDOWNLOADREQUEST";
                        message = "An operation could not be completed because a download request is required from the download handler.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80246005:
                    {
                        errorId = "WU_E_DM_NONETWORK";
                        message = "A download manager operation could not be completed because the network connection was unavailable.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80246006:
                    {
                        errorId = "WU_E_DM_WRONGBITSVERSION";
                        message = "A download manager operation could not be completed because the version of Background Intelligent Transfer Service (BITS) is incompatible.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80246007:
                    {
                        errorId = "WU_E_DM_NOTDOWNLOADED";
                        message = "The update has not been downloaded.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80246008:
                    {
                        errorId = "WU_E_DM_FAILTOCONNECTTOBITS";
                        message = "A download manager operation failed because the download manager was unable to connect the Background Intelligent Transfer Service (BITS).";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80246009:
                    {
                        errorId = "WU_E_DM_BITSTRANSFERERROR";
                        message = "A download manager operation failed because there was an unspecified Background Intelligent Transfer Service (BITS) transfer error.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024600A:
                    {
                        errorId = "WU_E_DM_DOWNLOADLOCATIONCHANGED";
                        message = "A download must be restarted because the location of the source of the download has changed.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x8024600B:
                    {
                        errorId = "WU_E_DM_CONTENTCHANGED";
                        message = "A download must be restarted because the update content changed in a new revision.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80246FFF:
                    {
                        errorId = "WU_E_DM_UNEXPECTED";
                        message = "There was a download manager error not covered by another WU_E_DM_* error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80242000:
                    {
                        errorId = "WU_E_UH_REMOTEUNAVAILABLE";
                        message = "A request for a remote update handler could not be completed because no remote process is available.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80242001:
                    {
                        errorId = "WU_E_UH_LOCALONLY";
                        message = "A request for a remote update handler could not be completed because the handler is local only.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x80242002:
                    {
                        errorId = "WU_E_UH_UNKNOWNHANDLER";
                        message = "A request for an update handler could not be completed because the handler could not be recognized.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80242003:
                    {
                        errorId = "WU_E_UH_REMOTEALREADYACTIVE";
                        message = "A remote update handler could not be created because one already exists.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80242004:
                    {
                        errorId = "WU_E_UH_DOESNOTSUPPORTACTION";
                        message = "A request for the handler to install (uninstall) an update could not be completed because the update does not support install (uninstall).";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80242005:
                    {
                        errorId = "WU_E_UH_WRONGHANDLER";
                        message = "An operation did not complete because the wrong handler was specified.";
                        category = ErrorCategory.InvalidArgument;
                        return true;
                    }
                case 0x80242006:
                    {
                        errorId = "WU_E_UH_INVALIDMETADATA";
                        message = "A handler operation could not be completed because the update contains invalid metadata.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80242007:
                    {
                        errorId = "WU_E_UH_INSTALLERHUNG";
                        message = "An operation could not be completed because the installer exceeded the time limit.";
                        category = ErrorCategory.OperationTimeout;
                        return true;
                    }
                case 0x80242008:
                    {
                        errorId = "WU_E_UH_OPERATIONCANCELLED";
                        message = "An operation being done by the update handler was canceled.";
                        category = ErrorCategory.OperationStopped;
                        return true;
                    }
                case 0x80242009:
                    {
                        errorId = "WU_E_UH_BADHANDLERXML";
                        message = "An operation could not be completed because the handler-specific metadata is invalid.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x8024200A:
                    {
                        errorId = "WU_E_UH_CANREQUIREINPUT";
                        message = "A request to the handler to install an update could not be completed because the update requires user input.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024200B:
                    {
                        errorId = "WU_E_UH_INSTALLERFAILURE";
                        message = "The installer failed to install (uninstall) one or more updates.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024200C:
                    {
                        errorId = "WU_E_UH_FALLBACKTOSELFCONTAINED";
                        message = "The update handler should download self-contained content rather than delta-compressed content for the update.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024200D:
                    {
                        errorId = "WU_E_UH_NEEDANOTHERDOWNLOAD";
                        message = "The update handler did not install the update because it needs to be downloaded again.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x8024200E:
                    {
                        errorId = "WU_E_UH_NOTIFYFAILURE";
                        message = "The update handler failed to send notification of the status of the install (uninstall) operation.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x8024200F:
                    {
                        errorId = "WU_E_UH_INCONSISTENT_FILE_NAMES";
                        message = "The file names contained in the update metadata and in the update package are inconsistent.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80242010:
                    {
                        errorId = "WU_E_UH_FALLBACKERROR";
                        message = "The update handler failed to fall back to the self-contained content.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80242011:
                    {
                        errorId = "WU_E_UH_TOOMANYDOWNLOADREQUESTS";
                        message = "The update handler has exceeded the maximum number of download requests.";
                        category = ErrorCategory.QuotaExceeded;
                        return true;
                    }
                case 0x80242012:
                    {
                        errorId = "WU_E_UH_UNEXPECTEDCBSRESPONSE";
                        message = "The update handler has received an unexpected response from CBS.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80242013:
                    {
                        errorId = "WU_E_UH_BADCBSPACKAGEID";
                        message = "The update metadata contains an invalid CBS package identifier.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80242014:
                    {
                        errorId = "WU_E_UH_POSTREBOOTSTILLPENDING";
                        message = "The post-reboot operation for the update is still in progress.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x80242015:
                    {
                        errorId = "WU_E_UH_POSTREBOOTRESULTUNKNOWN";
                        message = "The result of the post-reboot operation for the update could not be determined.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80242016:
                    {
                        errorId = "WU_E_UH_POSTREBOOTUNEXPECTEDSTATE";
                        message = "The state of the update after its post-reboot operation has completed is unexpected.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80242017:
                    {
                        errorId = "WU_E_UH_NEW_SERVICING_STACK_REQUIRED";
                        message = "The OS servicing stack must be updated before this update is downloaded or installed.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80242FFF:
                    {
                        errorId = "WU_E_UH_UNEXPECTED";
                        message = "An update handler error not covered by another WU_E_UH_* code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80248000:
                    {
                        errorId = "WU_E_DS_SHUTDOWN";
                        message = "An operation failed because Windows Update Agent is shutting down.";
                        category = ErrorCategory.OperationStopped;
                        return true;
                    }
                case 0x80248001:
                    {
                        errorId = "WU_E_DS_INUSE";
                        message = "An operation failed because the data store was in use.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x80248002:
                    {
                        errorId = "WU_E_DS_INVALID";
                        message = "The current and expected states of the data store do not match.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80248003:
                    {
                        errorId = "WU_E_DS_TABLEMISSING";
                        message = "The data store is missing a table.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80248004:
                    {
                        errorId = "WU_E_DS_TABLEINCORRECT";
                        message = "The data store contains a table with unexpected columns.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80248005:
                    {
                        errorId = "WU_E_DS_INVALIDTABLENAME";
                        message = "A table could not be opened because the table is not in the data store.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80248006:
                    {
                        errorId = "WU_E_DS_BADVERSION";
                        message = "The current and expected versions of the data store do not match.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80248007:
                    {
                        errorId = "WU_E_DS_NODATA";
                        message = "The information requested is not in the data store.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80248008:
                    {
                        errorId = "WU_E_DS_MISSINGDATA";
                        message = "The data store is missing required information or has a NULL in a table column that requires a non-null value.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80248009:
                    {
                        errorId = "WU_E_DS_MISSINGREF";
                        message = "The data store is missing required information or has a reference to missing license terms file localized property or linked row.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024800A:
                    {
                        errorId = "WU_E_DS_UNKNOWNHANDLER";
                        message = "The update was not processed because its update handler could not be recognized.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024800B:
                    {
                        errorId = "WU_E_DS_CANTDELETE";
                        message = "The update was not deleted because it is still referenced by one or more services.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x8024800C:
                    {
                        errorId = "WU_E_DS_LOCKTIMEOUTEXPIRED";
                        message = "The data store section could not be locked within the allotted time.";
                        category = ErrorCategory.OperationTimeout;
                        return true;
                    }
                case 0x8024800D:
                    {
                        errorId = "WU_E_DS_NOCATEGORIES";
                        message = "The category was not added because it contains no parent categories and is not a top-level category itself.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024800E:
                    {
                        errorId = "WU_E_DS_ROWEXISTS";
                        message = "The row was not added because an existing row has the same primary key.";
                        category = ErrorCategory.ResourceExists;
                        return true;
                    }
                case 0x8024800F:
                    {
                        errorId = "WU_E_DS_STOREFILELOCKED";
                        message = "The data store could not be initialized because it was locked by another process.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x80248010:
                    {
                        errorId = "WU_E_DS_CANNOTREGISTER";
                        message = "The data store is not allowed to be registered with COM in the current process.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80248011:
                    {
                        errorId = "WU_E_DS_UNABLETOSTART";
                        message = "Could not create a data store object in another process.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80248013:
                    {
                        errorId = "WU_E_DS_DUPLICATEUPDATEID";
                        message = "The server sent the same update to the client with two different revision IDs.";
                        category = ErrorCategory.ResourceExists;
                        return true;
                    }
                case 0x80248014:
                    {
                        errorId = "WU_E_DS_UNKNOWNSERVICE";
                        message = "An operation did not complete because the service is not in the data store.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80248015:
                    {
                        errorId = "WU_E_DS_SERVICEEXPIRED";
                        message = "An operation did not complete because the registration of the service has expired.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x80248016:
                    {
                        errorId = "WU_E_DS_DECLINENOTALLOWED";
                        message = "A request to hide an update was declined because it is a mandatory update or because it was deployed with a deadline.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80248017:
                    {
                        errorId = "WU_E_DS_TABLESESSIONMISMATCH";
                        message = "A table was not closed because it is not associated with the session.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80248018:
                    {
                        errorId = "WU_E_DS_SESSIONLOCKMISMATCH";
                        message = "A table was not closed because it is not associated with the session.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80248019:
                    {
                        errorId = "WU_E_DS_NEEDWINDOWSSERVICE";
                        message = "A request to remove the Windows Update service or to unregister it with Automatic Updates was declined because it is a built-in service and/or Automatic Updates cannot fall back to another service.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024801A:
                    {
                        errorId = "WU_E_DS_INVALIDOPERATION";
                        message = "A request was declined because the operation is not allowed.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024801B:
                    {
                        errorId = "WU_E_DS_SCHEMAMISMATCH";
                        message = "The schema of the current data store and the schema of a table in a backup XML document do not match.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024801C:
                    {
                        errorId = "WU_E_DS_RESETREQUIRED";
                        message = "The data store requires a session reset; release the session and retry with a new session.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x8024801D:
                    {
                        errorId = "WU_E_DS_IMPERSONATED";
                        message = "A data store operation did not complete because it was requested with an impersonated identity.";
                        category = ErrorCategory.AuthenticationError;
                        return true;
                    }
                case 0x80248FFF:
                    {
                        errorId = "WU_E_DS_UNEXPECTED";
                        message = "A data store error not covered by another WU_E_DS_* code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024C001:
                    {
                        errorId = "WU_E_DRV_PRUNED";
                        message = "A driver was skipped.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024C002:
                    {
                        errorId = "WU_E_DRV_NOPROP_OR_LEGACY";
                        message = "A property for the driver could not be found. It may not conform with required specifications.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024C003:
                    {
                        errorId = "WU_E_DRV_REG_MISMATCH";
                        message = "The registry type read for the driver does not match the expected type.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024C004:
                    {
                        errorId = "WU_E_DRV_NO_METADATA";
                        message = "The driver update is missing metadata.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x8024C005:
                    {
                        errorId = "WU_E_DRV_MISSING_ATTRIBUTE";
                        message = "The driver update is missing a required attribute.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024C006:
                    {
                        errorId = "WU_E_DRV_SYNC_FAILED";
                        message = "Driver synchronization failed.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024C007:
                    {
                        errorId = "WU_E_DRV_NO_PRINTER_CONTENT";
                        message = "Information required for the synchronization of applicable printers is missing.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024CFFF:
                    {
                        errorId = "WU_E_DRV_UNEXPECTED";
                        message = "A driver error not covered by another WU_E_DRV_* code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80240001:
                    {
                        errorId = "WU_E_NO_SERVICE";
                        message = "Windows Update Agent was unable to provide the service.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x80240002:
                    {
                        errorId = "WU_E_MAX_CAPACITY_REACHED";
                        message = "The maximum capacity of the service was exceeded.";
                        category = ErrorCategory.QuotaExceeded;
                        return true;
                    }
                case 0x80240003:
                    {
                        errorId = "WU_E_UNKNOWN_ID";
                        message = "An ID cannot be found.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80240004:
                    {
                        errorId = "WU_E_NOT_INITIALIZED";
                        message = "The object could not be initialized.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80240005:
                    {
                        errorId = "WU_E_RANGEOVERLAP";
                        message = "The update handler requested a byte range overlapping a previously requested range.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80240006:
                    {
                        errorId = "WU_E_TOOMANYRANGES";
                        message = "The requested number of byte ranges exceeds the maximum number (2^31 - 1).";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80240007:
                    {
                        errorId = "WU_E_INVALIDINDEX";
                        message = "The index to a collection was invalid.";
                        category = ErrorCategory.InvalidArgument;
                        return true;
                    }
                case 0x80240008:
                    {
                        errorId = "WU_E_ITEMNOTFOUND";
                        message = "The key for the item queried could not be found.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80240009:
                    {
                        errorId = "WU_E_OPERATIONINPROGRESS";
                        message = "Another conflicting operation was in progress. Some operations such as installation cannot be performed twice simultaneously.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x8024000A:
                    {
                        errorId = "WU_E_COULDNOTCANCEL";
                        message = "Cancellation of the operation was not allowed.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x8024000B:
                    {
                        errorId = "WU_E_CALL_CANCELLED";
                        message = "Operation was canceled.";
                        category = ErrorCategory.OperationStopped;
                        return true;
                    }
                case 0x8024000C:
                    {
                        errorId = "WU_E_NOOP";
                        message = "No operation was required.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024000D:
                    {
                        errorId = "WU_E_XML_MISSINGDATA";
                        message = "Windows Update Agent could not find required information in the update's XML data.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024000E:
                    {
                        errorId = "WU_E_XML_INVALID";
                        message = "Windows Update Agent found invalid information in the update's XML data.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024000F:
                    {
                        errorId = "WU_E_CYCLE_DETECTED";
                        message = "Circular update relationships were detected in the metadata.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x80240010:
                    {
                        errorId = "WU_E_TOO_DEEP_RELATION";
                        message = "Update relationships too deep to evaluate were evaluated.";
                        category = ErrorCategory.LimitsExceeded;
                        return true;
                    }
                case 0x80240011:
                    {
                        errorId = "WU_E_INVALID_RELATIONSHIP";
                        message = "An invalid update relationship was detected.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80240012:
                    {
                        errorId = "WU_E_REG_VALUE_INVALID";
                        message = "An invalid registry value was read.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80240013:
                    {
                        errorId = "WU_E_DUPLICATE_ITEM";
                        message = "Operation tried to add a duplicate item to a list.";
                        category = ErrorCategory.ResourceExists;
                        return true;
                    }
                case 0x80240016:
                    {
                        errorId = "WU_E_INSTALL_NOT_ALLOWED";
                        message = "Operation tried to install while another installation was in progress or the system was pending a mandatory restart.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x80240017:
                    {
                        errorId = "WU_E_NOT_APPLICABLE";
                        message = "Operation was not performed because there are no applicable updates.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80240018:
                    {
                        errorId = "WU_E_NO_USERTOKEN";
                        message = "Operation failed because a required user token is missing.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x80240019:
                    {
                        errorId = "WU_E_EXCLUSIVE_INSTALL_CONFLICT";
                        message = "An exclusive update cannot be installed with other updates at the same time.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x8024001A:
                    {
                        errorId = "WU_E_POLICY_NOT_SET";
                        message = "A policy value was not set.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024001B:
                    {
                        errorId = "WU_E_SELFUPDATE_IN_PROGRESS";
                        message = "The operation could not be performed because the Windows Update Agent is self-updating.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x8024001D:
                    {
                        errorId = "WU_E_INVALID_UPDATE";
                        message = "An update contains invalid metadata.";
                        category = ErrorCategory.MetadataError;
                        return true;
                    }
                case 0x8024001E:
                    {
                        errorId = "WU_E_SERVICE_STOP";
                        message = "Operation did not complete because the service or system was being shut down.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x8024001F:
                    {
                        errorId = "WU_E_NO_CONNECTION";
                        message = "Operation did not complete because the network connection was unavailable.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x80240020:
                    {
                        errorId = "WU_E_NO_INTERACTIVE_USER";
                        message = "Operation did not complete because there is no logged-on interactive user.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80240021:
                    {
                        errorId = "WU_E_TIME_OUT";
                        message = "Operation did not complete because it timed out.";
                        category = ErrorCategory.OperationTimeout;
                        return true;
                    }
                case 0x80240022:
                    {
                        errorId = "WU_E_ALL_UPDATES_FAILED";
                        message = "Operation failed for all the updates.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80240023:
                    {
                        errorId = "WU_E_EULAS_DECLINED";
                        message = "The license terms for all updates were declined.";
                        category = ErrorCategory.InvalidArgument;
                        return true;
                    }
                case 0x80240024:
                    {
                        errorId = "WU_E_NO_UPDATE";
                        message = "There are no updates.";
                        category = ErrorCategory.ObjectNotFound;
                        return true;
                    }
                case 0x80240025:
                    {
                        errorId = "WU_E_USER_ACCESS_DISABLED";
                        message = "Group Policy settings prevented access to Windows Update.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x80240026:
                    {
                        errorId = "WU_E_INVALID_UPDATE_TYPE";
                        message = "The type of update is invalid.";
                        category = ErrorCategory.InvalidType;
                        return true;
                    }
                case 0x80240027:
                    {
                        errorId = "WU_E_URL_TOO_LONG";
                        message = "The URL exceeded the maximum length.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80240028:
                    {
                        errorId = "WU_E_UNINSTALL_NOT_ALLOWED";
                        message = "The update could not be uninstalled because the request did not originate from a WSUS server.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x80240029:
                    {
                        errorId = "WU_E_INVALID_PRODUCT_LICENSE";
                        message = "Search may have missed some updates before there is an unlicensed application on the system.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024002A:
                    {
                        errorId = "WU_E_MISSING_HANDLER";
                        message = "A component required to detect applicable updates was missing.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x8024002B:
                    {
                        errorId = "WU_E_LEGACYSERVER";
                        message = "An operation did not complete because it requires a newer version of server.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x8024002C:
                    {
                        errorId = "WU_E_BIN_SOURCE_ABSENT";
                        message = "A delta-compressed update could not be installed because it required the source.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024002D:
                    {
                        errorId = "WU_E_SOURCE_ABSENT";
                        message = "A full-file update could not be installed because it required the source.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024002E:
                    {
                        errorId = "WU_E_WU_DISABLED";
                        message = "Access to an unmanaged server is not allowed.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x8024002F:
                    {
                        errorId = "WU_E_CALL_CANCELLED_BY_POLICY";
                        message = "Operation did not complete because the DisableWindowsUpdateAccess policy was set.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x80240030:
                    {
                        errorId = "WU_E_INVALID_PROXY_SERVER";
                        message = "The format of the proxy list was invalid.";
                        category = ErrorCategory.InvalidArgument;
                        return true;
                    }
                case 0x80240031:
                    {
                        errorId = "WU_E_INVALID_FILE";
                        message = "The file is in the wrong format.";
                        category = ErrorCategory.InvalidArgument;
                        return true;
                    }
                case 0x80240032:
                    {
                        errorId = "WU_E_INVALID_CRITERIA";
                        message = "The search criteria string was invalid.";
                        category = ErrorCategory.InvalidArgument;
                        return true;
                    }
                case 0x80240033:
                    {
                        errorId = "WU_E_EULA_UNAVAILABLE";
                        message = "License terms could not be downloaded.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80240034:
                    {
                        errorId = "WU_E_DOWNLOAD_FAILED";
                        message = "Update failed to download.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80240035:
                    {
                        errorId = "WU_E_UPDATE_NOT_PROCESSED";
                        message = "The update was not processed.";
                        category = ErrorCategory.InvalidResult;
                        return true;
                    }
                case 0x80240036:
                    {
                        errorId = "WU_E_INVALID_OPERATION";
                        message = "The object's current state did not allow the operation.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80240037:
                    {
                        errorId = "WU_E_NOT_SUPPORTED";
                        message = "The functionality for the operation is not supported.";
                        category = ErrorCategory.NotImplemented;
                        return true;
                    }
                case 0x80240038:
                    {
                        errorId = "WU_E_WINHTTP_INVALID_FILE";
                        message = "The downloaded file has an unexpected content type.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x80240039:
                    {
                        errorId = "WU_E_TOO_MANY_RESYNC";
                        message = "Agent is asked by server to resync too many times.";
                        category = ErrorCategory.QuotaExceeded;
                        return true;
                    }
                case 0x80240040:
                    {
                        errorId = "WU_E_NO_SERVER_CORE_SUPPORT";
                        message = "WUA API method does not run on Server Core installation.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80240041:
                    {
                        errorId = "WU_E_SYSPREP_IN_PROGRESS";
                        message = "Service is not available while sysprep is running.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x80240042:
                    {
                        errorId = "WU_E_UNKNOWN_SERVICE";
                        message = "The update service is no longer registered with AU.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x80240043:
                    {
                        errorId = "WU_E_NO_UI_SUPPORT";
                        message = "There is no support for WUA UI.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x80240FFF:
                    {
                        errorId = "WU_E_UNEXPECTED";
                        message = "An operation failed due to reasons not covered by another error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80070422:
                    {
                        // error id not documented for this error code
                        errorId = "WUServiceUnavailable";
                        message = "Windows Update service stopped working or is not running.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x00240001:
                    {
                        errorId = "WU_S_SERVICE_STOP";
                        message = "Windows Update Agent was stopped successfully.";
                        category = ErrorCategory.OperationStopped;
                        return true;
                    }
                case 0x00240002:
                    {
                        errorId = "WU_S_SELFUPDATE";
                        message = "Windows Update Agent updated itself.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x00240003:
                    {
                        errorId = "WU_S_UPDATE_ERROR";
                        message = "Operation completed successfully but there were errors applying the updates.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x00240004:
                    {
                        errorId = "WU_S_MARKED_FOR_DISCONNECT";
                        message = "A callback was marked to be disconnected later because the request to disconnect the operation came while a callback was executing.";
                        category = ErrorCategory.ConnectionError;
                        return true;
                    }
                case 0x00240005:
                    {
                        errorId = "WU_S_REBOOT_REQUIRED";
                        message = "The system must be restarted to complete installation of the update.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x00240006:
                    {
                        errorId = "WU_S_ALREADY_INSTALLED";
                        message = "The update to be installed is already installed on the system.";
                        category = ErrorCategory.ResourceExists;
                        return true;
                    }
                case 0x00240007:
                    {
                        errorId = "WU_S_ALREADY_UNINSTALLED";
                        message = "The update to be removed is not installed on the system.";
                        category = ErrorCategory.NotInstalled;
                        return true;
                    }
                case 0x00240008:
                    {
                        errorId = "WU_S_ALREADY_DOWNLOADED";
                        message = "The update to be downloaded has already been downloaded.";
                        category = ErrorCategory.ResourceExists;
                        return true;
                    }
                case 0x80241001:
                    {
                        errorId = "WU_E_MSI_WRONG_VERSION";
                        message = "Search may have missed some updates because the Windows Installer is less than version 3.1.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80241002:
                    {
                        errorId = "WU_E_MSI_NOT_CONFIGURED";
                        message = "Search may have missed some updates because the Windows Installer is not configured.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80241003:
                    {
                        errorId = "WU_E_MSP_DISABLED";
                        message = "Search may have missed some updates because policy has disabled Windows Installer patching.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80241004:
                    {
                        errorId = "WU_E_MSI_WRONG_APP_CONTEXT";
                        message = "An update could not be applied because the application is installed per-user.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x80241FFF:
                    {
                        errorId = "WU_E_MSP_UNEXPECTED";
                        message = "Search may have missed some updates because there was a failure of the Windows Installer.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024D001:
                    {
                        errorId = "WU_E_SETUP_INVALID_INFDATA";
                        message = "Windows Update Agent could not be updated because an INF file contains invalid information.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024D002:
                    {
                        errorId = "WU_E_SETUP_INVALID_IDENTDATA";
                        message = "Windows Update Agent could not be updated because the wuident.cab file contains invalid information.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024D003:
                    {
                        errorId = "WU_E_SETUP_ALREADY_INITIALIZED";
                        message = "Windows Update Agent could not be updated because of an internal error that caused setup initialization to be performed twice.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024D004:
                    {
                        errorId = "WU_E_SETUP_NOT_INITIALIZED";
                        message = "Windows Update Agent could not be updated because setup initialization never completed successfully.";
                        category = ErrorCategory.ResourceUnavailable;
                        return true;
                    }
                case 0x8024D005:
                    {
                        errorId = "WU_E_SETUP_SOURCE_VERSION_MISMATCH";
                        message = "Windows Update Agent could not be updated because the versions specified in the INF do not match the actual source file versions.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024D006:
                    {
                        errorId = "WU_E_SETUP_TARGET_VERSION_GREATER";
                        message = "Windows Update Agent could not be updated because a WUA file on the target system is newer than the corresponding source file.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024D007:
                    {
                        errorId = "WU_E_SETUP_REGISTRATION_FAILED";
                        message = "Windows Update Agent could not be updated because regsvr32.exe returned an error.";
                        category = ErrorCategory.FromStdErr;
                        return true;
                    }
                case 0x8024D009:
                    {
                        errorId = "WU_E_SETUP_SKIP_UPDATE";
                        message = "An update to the Windows Update Agent was skipped due to a directive in the wuident.cab file.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024D00A:
                    {
                        errorId = "WU_E_SETUP_UNSUPPORTED_CONFIGURATION";
                        message = "Windows Update Agent could not be updated because the current system configuration is not supported.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024D00B:
                    {
                        errorId = "WU_E_SETUP_BLOCKED_CONFIGURATION";
                        message = "Windows Update Agent could not be updated because the system is configured to block the update.";
                        category = ErrorCategory.PermissionDenied;
                        return true;
                    }
                case 0x8024D00C:
                    {
                        errorId = "WU_E_SETUP_REBOOT_TO_FIX";
                        message = "Windows Update Agent could not be updated because a restart of the system is required.";
                        category = ErrorCategory.InvalidOperation;
                        return true;
                    }
                case 0x8024D00D:
                    {
                        errorId = "WU_E_SETUP_ALREADYRUNNING";
                        message = "Windows Update Agent setup is already running.";
                        category = ErrorCategory.ResourceBusy;
                        return true;
                    }
                case 0x8024D00E:
                    {
                        errorId = "WU_E_SETUP_REBOOTREQUIRED";
                        message = "Windows Update Agent setup package requires a reboot to complete installation.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024D00F:
                    {
                        errorId = "WU_E_SETUP_HANDLER_EXEC_FAILURE";
                        message = "Windows Update Agent could not be updated because the setup handler failed during execution.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024D010:
                    {
                        errorId = "WU_E_SETUP_INVALID_REGISTRY_DATA";
                        message = "Windows Update Agent could not be updated because the registry contains invalid information.";
                        category = ErrorCategory.InvalidData;
                        return true;
                    }
                case 0x8024D013:
                    {
                        errorId = "WU_E_SETUP_WRONG_SERVER_VERSION";
                        message = "Windows Update Agent could not be updated because the server does not contain update information for this version.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                case 0x8024DFFF:
                    {
                        errorId = "WU_E_SETUP_UNEXPECTED";
                        message = "Windows Update Agent could not be updated because of an error not covered by another WU_E_SETUP_* error code.";
                        category = ErrorCategory.NotSpecified;
                        return true;
                    }
                default:
                    {
                        errorId = "ComException";
                        category = ErrorCategory.NotSpecified;
                        message = $"COM exception {errorCode:X8} was thrown.";
                        return false;
                    }
            }
        }
    }
}