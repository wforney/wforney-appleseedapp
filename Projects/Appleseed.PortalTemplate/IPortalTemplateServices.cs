using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appleseed.PortalTemplate.DTOs;

namespace Appleseed.PortalTemplate
{
    public interface IPortalTemplateServices
    {
        bool SerializePortal(int portalID, string path);

        bool DeserializePortal(string file, string portalName, string portalAlias, string portalPath, out int portalId);

        HtmlTextDTO GetHtmlTextDTO(int moduleId);

        bool SaveHtmlText(int moduleId, HtmlTextDTO html);
    }
}
