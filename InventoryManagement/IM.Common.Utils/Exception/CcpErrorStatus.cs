namespace IM.Common.Utils.Exception
{
    public enum CcpErrorStatus
    {
        /// <summary>
        ///     Not an error. Return on Success.
        /// </summary>
        [HttpCode(200)]
        [Description(ErrorConstants.ERRORSTATUS_OK)]
        OK = 20000,

        /// <summary>
        ///     Not an error. Return during POST or PUT.
        /// </summary>
        [HttpCode(201)]
        [Description(ErrorConstants.ERRORSTATUS_CREATED)]
        Created = 20100,

        /// <summary>
        ///     Not an error. Return during POST or PUT.
        /// </summary>
        [HttpCode(202)]
        [Description(ErrorConstants.ERRORSTATUS_ACCEPTED)]
        Accepted = 20200,

        /// <summary>
        ///     Not an error. Return during POST or PUT.
        /// </summary>
        [HttpCode(204)]
        [Description(ErrorConstants.ERRORSTATUS_NOCONTENT)]
        NoContent = 20400,

        /// <summary>
        ///     Not an error. Return during POST or PUT.
        /// </summary>
        [HttpCode(206)]
        [Description(ErrorConstants.ERRORSTATUS_PARTIALCONTENT)]
        PartialContent = 20600,

        /// <summary>
        ///     The client specified an invalid argument.  Note that this differs
        ///     from `FAILED_PRECONDITION`.  `INVALID_ARGUMENT` indicates arguments
        ///     that are problematic regardless of the state of the system
        ///     (e.g., a malformed file name).
        ///     HTTP Mapping: 400 Bad Request
        /// </summary>
        [HttpCode(400)]
        [Description(ErrorConstants.ERRORSTATUS_INVALIDARGUMENT)]
        InvalidArgument = 40000,

        /// <summary>
        ///     The operation was rejected because the system is not in a state
        ///     required for the operation's execution.  For example, the directory
        ///     to be deleted is non-empty, an rmdir operation is applied to
        ///     a non-directory, etc.
        ///     Service implementors can use the following guidelines to decide
        ///     between `FAILED_PRECONDITION`, `ABORTED`, and `UNAVAILABLE`:
        ///     (a) Use `UNAVAILABLE` if the client can retry just the failing call.
        ///     (b) Use `ABORTED` if the client should retry at a higher level
        ///     (e.g., when a client-specified test-and-set fails, indicating the
        ///     client should restart a read-modify-write sequence).
        ///     (c) Use `FAILED_PRECONDITION` if the client should not retry until
        ///     the system state has been explicitly fixed.  E.g., if an "rmdir"
        ///     fails because the directory is non-empty, `FAILED_PRECONDITION`
        ///     should be returned since the client should not retry unless
        ///     the files are deleted from the directory.
        ///     HTTP Mapping: 400 Bad Request
        /// </summary>
        [HttpCode(400)]
        [Description(ErrorConstants.ERRORSTATUS_FAILEDPRECONDITION)]
        FailedPrecondition = 40001,

        [HttpCode(400)]
        [Description(ErrorConstants.ERRORSTATUS_INVALIDINPUTS)]
        InvalidInputs = 40002,

        /// <summary>
        ///     The operation was attempted past the valid range.  E.g., seeking or
        ///     reading past end-of-file.
        ///     Unlike `INVALID_ARGUMENT`, this error indicates a problem that may
        ///     be fixed if the system state changes. For example, a 32-bit file
        ///     system will generate `INVALID_ARGUMENT` if asked to read at an
        ///     offset that is not in the range [0,2^32-1], but it will generate
        ///     `OUT_OF_RANGE` if asked to read from an offset past the current
        ///     file size.
        ///     There is a fair bit of overlap between `FAILED_PRECONDITION` and
        ///     `OUT_OF_RANGE`.  We recommend using `OUT_OF_RANGE` (the more specific
        ///     error) when it applies so that callers who are iterating through
        ///     a space can easily look for an `OUT_OF_RANGE` error to detect when
        ///     they are done.
        ///     HTTP Mapping: 400 Bad Request
        /// </summary>
        [HttpCode(400)]
        [Description(ErrorConstants.ERRORSTATUS_OUTOFRANGE)]
        OutOfRange = 40003,

        /// <summary>
        ///     The request does not have valid authentication credentials for the operation.
        ///     HTTP Mapping: 401 Unauthorized
        /// </summary>
        [HttpCode(401)]
        [Description(ErrorConstants.ERRORSTATUS_UNAUTHENTICATED)]
        Unauthenticated = 40100,

        /// <summary>
        ///     The caller does not have permission to execute the specified
        ///     operation. `PERMISSION_DENIED` must not be used for rejections
        ///     caused by exhausting some resource (use `RESOURCE_EXHAUSTED`
        ///     instead for those errors). `PERMISSION_DENIED` must not be
        ///     used if the caller can not be identified (use `UNAUTHENTICATED`
        ///     instead for those errors). This error code does not imply the
        ///     request is valid or the requested entity exists or satisfies
        ///     other pre-conditions.
        ///     HTTP Mapping: 403 Forbidden
        /// </summary>
        [HttpCode(403)]
        [Description(ErrorConstants.ERRORSTATUS_PERMISSIONDENIED)]
        PermissionDenied = 40300,

        /// <summary>
        ///     Some requested entity (e.g., file or directory) was not found.
        ///     Note to server developers: if a request is denied for an entire class
        ///     of users, such as gradual feature rollout or undocumented whitelist,
        ///     `NOT_FOUND` may be used. If a request is denied for some users within
        ///     a class of users, such as user-based access control, `PERMISSION_DENIED`
        ///     must be used.
        ///     HTTP Mapping: 404 Not Found
        /// </summary>
        [HttpCode(404)]
        [Description(ErrorConstants.ERRORSTATUS_NOTFOUND)]
        NotFound = 40400,

        /// <summary>
        ///     The entity that a client attempted to create already exists.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_ALREADYEXISTS)]
        AlreadyExists = 40900,

        /// <summary>
        ///     The operation was aborted, typically due to a concurrency issue such as
        ///     a sequencer check failure or transaction abort.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_ABORTED)]
        Aborted = 40901,

        /// <summary>
        ///     Request attempted to update an entity that does not exist.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_DOESNOTEXIST)]
        DoesNotExist = 40902,

        /// <summary>
        ///     Request attempted to approve a user who is internal.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_USERINTERNAL)]
        UserisInternal = 40903,

        /// <summary>
        ///     Request attempted to approve a user who is already approved.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_APPROVEDUSER)]
        ApprovedUser = 40904,


        /// <summary>
        ///     Request attempted on user who is not approved.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_NOTAPPROVEDUSER)]
        NotApprovedUser = 40905,

        /// <summary>
        ///     Request attempted on user who is already activated.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_ACTIVATEDUSER)]
        ActivatedUser = 40906,

        /// <summary>
        ///     Request attempted on user who is already inactive.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_INACTIVEUSER)]
        InactiveUser = 40907,

        /// <summary>
        ///     Request attempted on user who is already deleted.
        ///     HTTP Mapping: 409 Conflict
        /// </summary>
        [HttpCode(409)]
        [Description(ErrorConstants.ERRORSTATUS_DELETEDUSER)]
        DeletedUser = 40908,


        [HttpCode(415)]
        [Description(ErrorConstants.ERRORSTATUS_INVALIDCONTENTTYPE)]
        InvalidContentType = 41500,

        [HttpCode(422)]
        [Description(ErrorConstants.ERRORSTATUS_UNPROCESSABLEENTITY)]
        UnprocessableEntity = 42200,

        /// <summary>
        ///     Some resource has been exhausted, perhaps a per-user quota, or
        ///     perhaps the entire file system is out of space.
        ///     HTTP Mapping: 429 Too Many Requests
        /// </summary>
        [HttpCode(429)]
        [Description(ErrorConstants.ERRORSTATUS_RESOURCEEXHAUSTED)]
        ResourceExhausted = 42900,

        /// <summary>
        ///     The operation was cancelled, typically by the caller.
        ///     HTTP Mapping: 499 Client Closed Request
        /// </summary>
        [HttpCode(499)]
        [Description(ErrorConstants.ERRORSTATUS_CANCELLED)]
        Cancelled = 49900,

        /// <summary>
        ///     Unknown error.  For example, this error may be returned when
        ///     a `Status` value received from another address space belongs to
        ///     an error space that is not known in this address space.  Also
        ///     errors raised by APIs that do not return enough error information
        ///     may be converted to this error.
        ///     HTTP Mapping: 500 Internal Server Error
        /// </summary>
        [HttpCode(500)]
        [Description(ErrorConstants.ERRORSTATUS_UNKNOWN)]
        Unknown = 50000,

        /// <summary>
        ///     Internal errors.  This means that some invariants expected by the
        ///     underlying system have been broken.  This error code is reserved
        ///     for serious errors.
        ///     HTTP Mapping: 500 Internal Server Error
        /// </summary>
        [HttpCode(500)]
        [Description(ErrorConstants.ERRORSTATUS_INTERNAL)]
        Internal = 50001,

        /// <summary>
        ///     Unrecoverable data loss or corruption.
        ///     HTTP Mapping: 500 Internal Server Error
        [HttpCode(500)]
        [Description(ErrorConstants.ERRORSTATUS_DATALOSS)]
        DataLoss = 50002,

        /// <summary>
        ///     The operation is not implemented or is not supported/enabled in this
        ///     service.
        ///     HTTP Mapping: 501 Not Implemented
        /// </summary>
        [HttpCode(501)]
        [Description(ErrorConstants.ERRORSTATUS_UNIMPLEMENTED)]
        Unimplemented = 50100,

        /// <summary>
        ///     The service is currently unavailable.  This is most likely a
        ///     transient condition, which can be corrected by retrying with
        ///     a backoff.
        ///     See the guidelines above for deciding between `FAILED_PRECONDITION`,
        ///     `ABORTED`, and `UNAVAILABLE`.
        ///     HTTP Mapping: 503 Service Unavailable
        /// </summary>
        [HttpCode(503)]
        [Description(ErrorConstants.ERRORSTATUS_UNAVAILABLE)]
        Unavailable = 50300,

        /// <summary>
        ///     The deadline expired before the operation could complete. For operations
        ///     that change the state of the system, this error may be returned
        ///     even if the operation has completed successfully.  For example, a
        ///     successful response from a server could have been delayed long
        ///     enough for the deadline to expire.
        ///     HTTP Mapping: 504 Gateway Timeout
        /// </summary>
        [HttpCode(504)]
        [Description(ErrorConstants.ERRORSTATUS_DEADLINEEXCEEDED)]
        DeadlineExceeded = 50400
    }
}