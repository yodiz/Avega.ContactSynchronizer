/* Copyright (c) 2006 Google Inc.
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
using System.Xml;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using Google.GData.Client;
using Google.GData.Extensions;

namespace Google.GData.AccessControl 
{

    /// <summary>
    /// GData schema extension describing an account role
    /// </summary>
    public class AclScope : SimpleAttribute
    {
        /// <summary>string constant for the user scope</summary>
        public const string SCOPE_USER =  "user";
        /// <summary>string constant for the user scope</summary>
        public const string SCOPE_DOMAIN  =  "domain";
        /// <summary>string constant for the user scope</summary>
        public const string SCOPE_DEFAULT =  "default";

        /// <summary>
        /// default constructor
        /// </summary>
        public AclScope() : base(AclNameTable.XmlAclScopeElement,
                                         AclNameTable.gAclAlias,
                                        AclNameTable.gAclNamespace)
        {
            this.Attributes.Add(AclNameTable.XmlAttributeType, null);
        }

        /// <summary>
        /// default constructor with an init value
        /// </summary>
        /// <param name="initValue"></param>
        public AclScope(string initValue) : base(GDataParserNameTable.XmlExtendedPropertyElement,
                                         BaseNameTable.gDataPrefix,
                                         BaseNameTable.gNamespace,
                                         initValue)
        {
            this.Attributes.Add(AclNameTable.XmlAttributeType, null);
        }

        /// <summary>
        /// constructor taking an initial value and a name
        /// </summary>
        /// <param name="initValue"></param>
        /// <param name="initName"></param>
        public AclScope(string initValue, string initName) : base(GDataParserNameTable.XmlExtendedPropertyElement,
                                         BaseNameTable.gDataPrefix,
                                         BaseNameTable.gNamespace,
                                         initValue)
        {
            this.Attributes.Add(AclNameTable.XmlAttributeType, initName);
        }



        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method public string Type</summary> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        public String Type
        {
            get {return this.Attributes[AclNameTable.XmlAttributeType] as string;}
            set {
                if (String.Compare(value, SCOPE_USER) == 0 ||
                    String.Compare(value, SCOPE_DOMAIN) == 0 || 
                    String.Compare(value, SCOPE_DEFAULT) == 0)
                {
                    this.Attributes[AclNameTable.XmlAttributeType] = value;
                    if (String.Compare(value, SCOPE_DEFAULT) == 0)
                    {
                        // empty the value
                        this.Value = String.Empty;
                    }
                }
                else 
                {
                    throw new System.ArgumentException("Type should be one of the predifined values", value);
                }
            }
        }
        // end of accessor public string Type
    }
}
