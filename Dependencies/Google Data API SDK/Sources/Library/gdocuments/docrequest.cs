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
using System.IO;
using System.Collections;
using System.Text;
using System.Net; 
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.AccessControl;
using Google.GData.Documents;
using System.Collections.Generic;

namespace Google.Documents
{

    /// <summary>
    ///  the base class for all documents in the document service. A document can represent folders, documents, spreadsheets etc. 
    /// </summary>
    public class Document : Entry
    {

        /// <summary>
        /// descripes the type of the document entry
        /// </summary>
        public enum DocumentType
        {
            /// <summary>
            /// a docuemtn
            /// </summary>
            Document,
            /// <summary>
            /// a spreadsheet
            /// </summary>
            Spreadsheet,
            /// <summary>
            /// a pdf file
            /// </summary>
            PDF,
            /// <summary>
            /// a presentation
            /// </summary>
            Presentation,
            /// <summary>
            /// a folder
            /// </summary>
            Folder,
            /// <summary>
            /// something unknown to us
            /// </summary>
            Unknown
        }

        /// <summary>
        /// describes the download type, in what format you want to download the document
        /// </summary>
        public enum DownloadType
        {
            /// <summary>
            /// text file
            /// </summary>
            txt,
            /// <summary>
            /// open document format
            /// </summary>
            odt,
            /// <summary>
            /// portable document format PFDF
            /// </summary>
            pdf,
            /// <summary>
            /// html format
            /// </summary>
            html,
            /// <summary>
            /// rich text format
            /// </summary>
            rtf,
            /// <summary>
            /// microsoft word format
            /// </summary>
            doc,
            /// <summary>
            /// portable network graphics format
            /// </summary>
            png,
            /// <summary>
            /// flash format
            /// </summary>
            swf,
            /// <summary>
            /// Microsoft Powerpoint format
            /// </summary>
            ppt,
            /// <summary>
            /// Microsoft Excel format
            /// </summary>
            xls,
            /// <summary>
            /// commma seperated value format
            /// </summary>
            csv,
            /// <summary>
            /// open document spreadsheet format
            /// </summary>
            ods,
            /// <summary>
            /// tab seperated values format
            /// </summary>
            tsv
        }

        /// <summary>
        /// creates the inner contact object when needed
        /// </summary>
        /// <returns></returns>
        protected override void EnsureInnerObject()
        {
            if (this.AtomEntry == null)
            {
                this.AtomEntry = new DocumentEntry();
            }
        }



        /// <summary>
        /// readonly accessor for the DocumentEntry that is underneath this object.
        /// </summary>
        /// <returns></returns>
        public  DocumentEntry DocumentEntry
        {
            get
            {
                EnsureInnerObject();
                return this.AtomEntry as DocumentEntry;
            }
        }

        /// <summary>
        /// the type of the document entry
        /// </summary>
        /// <returns></returns>
        public Document.DocumentType Type
        {
            get
            {
                EnsureInnerObject();
                if (this.DocumentEntry.IsDocument)
                {
                    return Document.DocumentType.Document;
                } 
                else if (this.DocumentEntry.IsPDF)
                {
                    return Document.DocumentType.PDF;
                }
                else if (this.DocumentEntry.IsSpreadsheet)
                {
                    return Document.DocumentType.Spreadsheet;
                }
                else if (this.DocumentEntry.IsFolder)
                {
                    return Document.DocumentType.Folder;
                }
                else if (this.DocumentEntry.IsPresentation)
                {
                    return Document.DocumentType.Presentation;
                }
                return Document.DocumentType.Unknown;
            }
            set
            {
                EnsureInnerObject();
                switch (value)
                {
                    case Document.DocumentType.Document:
                        this.DocumentEntry.IsDocument = true;
                        break;
                    case Document.DocumentType.Folder:
                        this.DocumentEntry.IsFolder = true;
                        break;
                    case Document.DocumentType.PDF:
                        this.DocumentEntry.IsPDF = true;
                        break;
                    case Document.DocumentType.Presentation:
                        this.DocumentEntry.IsPresentation = true;
                        break;
                    case Document.DocumentType.Spreadsheet:
                        this.DocumentEntry.IsSpreadsheet = true;
                        break;
                    case Document.DocumentType.Unknown:
                        throw new ArgumentException("Type.Unknown is not allowed to be set");
                }
            }
        }

        /// <summary>
        /// returns the href values of the parent link releationships
        /// can be used to retrieve the parent folder
        /// </summary>
        /// <returns></returns>
        public List<string> ParentFolders
        {
            get
            {
                EnsureInnerObject();
                List<string> strings = new List<string>();
                foreach (AtomLink l in this.DocumentEntry.ParentFolders)
                {
                    strings.Add(l.HRef.ToString());
                }
                return strings;
            }
        }

        /// <summary>
        /// returns the document id of the object
        /// </summary>
        /// <returns></returns>
        public string DocumentId
        {
            get
            {
                EnsureInnerObject();
                return DocumentsListQuery.DocumentId(this.Id);
            }
        }
    }


    //////////////////////////////////////////////////////////////////////
    /// <summary>
    /// The Google Documents List Data API allows client applications 
    /// to view and update documents (spreadsheets and word processor) 
    /// using a Google Data API feed. Your client application can request
    /// a list of a user's documents, query the content of a 
    /// user's documents, and upload new documents.
    /// </summary>
    ///  <example>
    ///         The following code illustrates a possible use of   
    ///          the <c>DocumentsRequest</c> object:  
    ///          <code>    
    ///            RequestSettings settings = new RequestSettings("yourApp");
    ///            settings.PageSize = 50; 
    ///            settings.AutoPaging = true;
    ///            DocumentsRequest c = new DocumentsRequest(settings);
    ///            Feed&lt;Dcouments&gt; feed = c.GetDocuments();
    ///     
    ///         foreach (Document d in feed.Entries)
    ///         {
    ///              Console.WriteLine(d.Title);
    ///         }
    ///  </code>
    ///  </example>
    //////////////////////////////////////////////////////////////////////
    public class DocumentsRequest : FeedRequest<DocumentsService>
    {

        private string baseUri = DocumentsListQuery.documentsBaseUri;
        private string folderUri = DocumentsListQuery.folderBaseUri;
        private Service spreadsheetsService; 
        /// <summary>
        /// default constructor for a DocumentsRequest
        /// </summary>
        /// <param name="settings"></param>
        public DocumentsRequest(RequestSettings settings) : base(settings)
        {
            this.Service = new DocumentsService(settings.Application);
            // we hardcode the service name here to avoid having a dependency 
            // on the spreadsheet dll for now.
            this.spreadsheetsService = new Service("wise", settings.Application);
            this.spreadsheetsService.ProtocolMajor = 2;
            PrepareService();
            PrepareService(this.spreadsheetsService);
        }

        /// <summary>
        /// the base string to use for queries. Defaults to 
        /// DocumentsListQuery.documentsBaseUri
        /// </summary>
        /// <returns></returns>
        public string BaseUri
        {
            get
            {
                return this.baseUri;
            }
            set
            {
                this.BaseUri = value;
            }
        }

        /// <summary>
        /// the base string to use for folder queries. Defaults to 
        /// DocumentsListQuery.folderBaseUri
        /// </summary>
        /// <returns></returns>
        public string FolderUri
        {
            get
            {
                return this.folderUri;
            }
            set
            {
                this.folderUri = value;
            }
        }

        /// <summary>
        /// returns a Feed of all documents and folders for the authorized user
        /// </summary>
        /// <returns>a feed of everyting</returns>
        public Feed<Document> GetEverything()
        {
            DocumentsListQuery q = PrepareQuery<DocumentsListQuery>(this.BaseUri);
            q.ShowFolders = true; 
            return PrepareFeed<Document>(q); 
        }

        /// <summary>
        /// returns a Feed of all documents for the authorized user
        /// </summary>
        /// <returns>a feed of Documents</returns>
        public Feed<Document> GetDocuments()
        {
            TextDocumentQuery q = PrepareQuery<TextDocumentQuery>(this.BaseUri);
            return PrepareFeed<Document>(q); 
        }

        /// <summary>
        /// returns a Feed of all presentations for the authorized user
        /// </summary>
        /// <returns>a feed of Documents</returns>
        public Feed<Document> GetPresentations()
        {
            PresentationsQuery q = PrepareQuery<PresentationsQuery>(this.BaseUri);
            return PrepareFeed<Document>(q); 
        }

        /// <summary>
        /// returns a Feed of all spreadsheets for the authorized user
        /// </summary>
        /// <returns>a feed of Documents</returns>
        public Feed<Document> GetSpreadsheets()
        {
            SpreadsheetQuery q = PrepareQuery<SpreadsheetQuery>(this.BaseUri);
            return PrepareFeed<Document>(q); 
        }

        /// <summary>
        /// returns a Feed of all pdf files for the authorized user
        /// </summary>
        /// <returns>a feed of Documents</returns>
        public Feed<Document> GetPDFs()
        {
            PDFsQuery q = PrepareQuery<PDFsQuery>(this.BaseUri);
            return PrepareFeed<Document>(q); 
        }

        /// <summary>
        /// returns a Feed of all folders for the authorized user
        /// </summary>
        /// <returns>a feed of Documents</returns>
        public Feed<Document> GetFolders()
        {
            FolderQuery q = PrepareQuery<FolderQuery>(this.BaseUri);
            return PrepareFeed<Document>(q); 
        }

        /// <summary>
        /// returns a feed of documents for the specified folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Feed<Document> GetFolderContent(Document folder)
        {
            if (folder.Type != Document.DocumentType.Folder)
            {
                throw new ArgumentException("The parameter folder is not a folder");
            }

            string uri = this.FolderUri + folder.DocumentId;

            DocumentsListQuery q = PrepareQuery<DocumentsListQuery>(uri);
            return PrepareFeed<Document>(q); 
        }



        /// <summary>
        /// this will create an empty document or folder, pending
        /// the content of the newDocument parameter
        /// </summary>
        /// <param name="newDocument"></param>
        /// <returns>the created document from the server</returns>
        public Document CreateDocument(Document newDocument)
        {
            return Insert(new Uri(DocumentsListQuery.documentsBaseUri), newDocument);
        }

        /// <summary>
        /// moves a document or a folder into a folder
        /// </summary>
        /// <param name="parent">this has to be a folder</param>
        /// <param name="child">can be a folder or a document</param>
        /// <returns></returns>
        public Document MoveDocumentTo(Document parent, Document child)
        {
            if (parent == null || child == null)
            {
                throw new ArgumentNullException("parent or child can not be null");
            }
            if (parent.AtomEntry.Content == null || parent.AtomEntry.Content.AbsoluteUri == null)
            {
                throw new ArgumentException("parent has no content uri");
            }
            if (parent.Type != Document.DocumentType.Folder)
            {
                throw new ArgumentException("wrong parent type");
            }

            Document payload = new Document();
            payload.DocumentEntry.Id = new AtomId(child.Id);
            payload.Type = child.Type;

            // to do that, we just need to post the CHILD 
            // against the URI of the parent
            return Insert(new Uri(parent.AtomEntry.Content.AbsoluteUri), payload);
        }


        /// <summary>
        /// downloads a document. 
        /// </summary>
        /// <param name="document">The document to download. It needs to have the document type set, as well as the id link</param>
        /// <param name="type">The output format of the document you want to download</param>
        /// <returns></returns>
        public Stream Download(Document document, Document.DownloadType type)
        {
            return this.Download(document, type, null, 0);
        }
        

        /// <summary>
        /// downloads a document. 
        /// </summary>
        /// <param name="document">The document to download. It needs to have the document type set, as well as the id link</param>
        /// <param name="type">The output format of the document you want to download</param>
        /// <param name="sheetNumber">When requesting a CSV or TSV file you must specify an additional parameter called 
        /// gid which indicates which grid, or sheet, you wish to get (the index is 0 based, so gid 1 
        /// actually refers to the second sheet sheet on a given spreadsheet). </param>
        /// <param name="baseDomain">if null, default is used. Otherwise needs to specify the domain to download from, ending with a slash</param>
        /// <returns></returns>
        public Stream Download(Document document, Document.DownloadType type, string baseDomain, int sheetNumber)
        {
            if (document.Type == Document.DocumentType.Unknown)
            {
                throw new ArgumentException("Document has an unknown type");
            }

            if (document.Type == Document.DocumentType.Folder)
            {
                throw new ArgumentException("Document is a folder, can not be downloaded");
            }

            // now figure out the parameters
            string queryUri = "";

            Service s = this.Service; 

            switch (document.Type)
            {
    
                case Document.DocumentType.Spreadsheet:
                    // spreadsheet has a different parameter
                    if (baseDomain == null)
                    {
                        baseDomain = "http://spreadsheets.google.com/";
                    }
                    queryUri = baseDomain + "feeds/download/spreadsheets/Export?key=" + document.DocumentId + "&fmcmd="; 
                    s = this.spreadsheetsService;
                    switch (type)
                    {
                        case Document.DownloadType.xls:
                            queryUri+="4";
                            break;
                        case Document.DownloadType.csv:
                            queryUri+="5&gid="+sheetNumber.ToString();
                            break;
                        case Document.DownloadType.pdf:
                            queryUri+="12";
                            break;
                        case Document.DownloadType.ods:
                            queryUri+="13";
                            break;
                        case Document.DownloadType.tsv:
                            queryUri+="23&gid="+sheetNumber.ToString();;
                            break;
                        case Document.DownloadType.html:
                            queryUri+="102";
                            break;
                        default:
                            throw new ArgumentException("type is invalid for a spreadsheet");

                    }
                    break;

                case Document.DocumentType.Presentation:
                    if (baseDomain == null)
                    {
                        baseDomain = "http://docs.google.com/";
                    }

                    queryUri = baseDomain + "feeds/download/presentations/Export?docID=" + document.DocumentId + "&exportFormat="; 
                    switch (type)
                    {
                        case Document.DownloadType.swf:
                            queryUri+="swf";
                            break;
                        case Document.DownloadType.pdf:
                            queryUri+="pdf";
                            break;
                        case Document.DownloadType.ppt:
                            queryUri+="ppt";
                            break;
                        default:
                            throw new ArgumentException("type is invalid for a presentation");
                    }
                    break;
                default:
                    if (baseDomain == null)
                    {
                        baseDomain = "http://docs.google.com/";
                    }

                    queryUri = baseDomain + "feeds/download/documents/Export?docID=" + document.DocumentId + "&exportFormat=" + type.ToString(); 
                    break;

            }

            Uri target = new Uri(queryUri);
            return s.Query(target);
        }
    }
}
