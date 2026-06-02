using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Contracts.File;
public record DownloadFileResponse(
    Guid fileId,
    byte[] fileContent,
    Guid parentFolderId
    );
