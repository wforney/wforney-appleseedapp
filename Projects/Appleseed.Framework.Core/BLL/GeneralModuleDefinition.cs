using System;
using System.Collections.Generic;
using System.Text;

namespace Appleseed.Framework.Core.BLL {
    public class GeneralModuleDefinition {

        private int generalModDefID;
        private string friendlyName;
        private string desktopSource;
        private string mobileSource;
        private string assemblyName;
        private string className;
        private bool admin;
        private bool searchable;

        public int GeneralModDefID {
            get {
                return generalModDefID;
            }
            set {
                generalModDefID = value;
            }
        }

        public string FriendlyName {
            get {
                return friendlyName;
            }
            set {
                friendlyName = value;
            }
        }

        public string DesktopSource {
            get {
                return desktopSource;
            }
            set {
                desktopSource = value;
            }
        }

        public string MobileSource {
            get {
                return mobileSource;
            }
            set {
                mobileSource = value;
            }
        }

        public string AssemblyName {
            get {
                return assemblyName;
            }
            set {
                assemblyName = value;
            }
        }

        public string ClassName {
            get {
                return className;
            }
            set {
                className = value;
            }
        }

        public bool Admin {
            get {
                return admin;
            }
            set {
                admin = value;
            }
        }
        
        public bool Searchable {
            get {
                return searchable;
            }
            set {
                searchable = value;
            }
        }
    }
}
