/* Copyright (c) 2007 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections;
using System.Xml;
using System.Text;
using Google.GData.Client;
using Google.GData.Extensions.Apps;

namespace Google.GData.Apps.GoogleMailSettings
{
    /// <summary>
    /// A Google Apps Google Mail Settings entry.
    /// </summary>
    public class GoogleMailSettingsEntry : AtomEntry
    {
        private PropertyCollection properties;

        /// <summary>
        /// Constructs a new <code>GoogleMailSettingsEntry</code> object.
        /// </summary>
        public GoogleMailSettingsEntry()
            : base()
        {
            GAppsExtensions.AddPropertyElementExtensions(this);
        }

        /// <summary>
        /// Properties accessor
        /// </summary>
        public PropertyCollection Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new PropertyCollection(this);
                }
                return properties;
            }
            set
            {
                this.Properties = value;
            }
        }

        /// <summary>
        /// Gets a PropertyElement by its Name
        /// </summary>
        /// <returns>a <code>PropertyElement</code> containing the results of the
        /// execution</returns>         
        public PropertyElement getPropertyByName(string name)
        {
            foreach (PropertyElement property in this.Properties)
            {
                if (property.Name.Equals(name))
                {
                    return property;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a the Value of a PropertyElement by its Name
        /// </summary>
        /// <returns>a string containing the results of the execution</returns>
        public string getPropetyValueByName(string name)
        {
            PropertyElement property = this.getPropertyByName(name);
            if (property != null)
                return property.Value;
            else
                return null;
        }

        /// <summary>
        /// Updates this GoogleMailSettingsEntry.
        /// </summary>
        /// <returns>the updated GoogleMailSettingsEntry</returns>
        public new GoogleMailSettingsEntry Update()
        {
            try
            {
                return base.Update() as GoogleMailSettingsEntry;
            }
            catch (GDataRequestException e)
            {
                AppsException a = AppsException.ParseAppsException(e);
                throw (a == null ? e : a);
            }
        }
    }
}
