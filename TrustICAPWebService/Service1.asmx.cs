using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace TrustICAPWebService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://193.92.14.138/TrustICAPWebService/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        public ServiceAuthHeader CustomSoapHeader;
        [WebMethod()]
        [SoapHeader("CustomSoapHeader")]
        public XmlDocument SendQuestion(string sInput)
        {
            //kanoume to authentication gia to service
           ServiceAuthHeaderValidation.Validate(CustomSoapHeader);

            string username = null;
            string password = null;
            string prefix = null;
            string NamspaceURI = null;

            XmlDocument doc = new XmlDocument();
            XmlDocument doc_out = new XmlDocument();
            string con_string = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(con_string);

            string con_string_trust = WebConfigurationManager.ConnectionStrings["TrustMainConnectionString"].ConnectionString;
            SqlConnection con_trust = new SqlConnection(con_string_trust);

            try
            {
                XmlDeclaration xmlindeclaration = null;
                xmlindeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.InsertBefore(xmlindeclaration, doc.DocumentElement);

                doc.LoadXml(sInput);
                //diavazoume ta username password gia kathe customer

                username = System.Configuration.ConfigurationManager.AppSettings["username"];
                password = System.Configuration.ConfigurationManager.AppSettings["password"];
                prefix = System.Configuration.ConfigurationManager.AppSettings["prefix"];
                NamspaceURI = System.Configuration.ConfigurationManager.AppSettings["NamspaceURI"];
                //FTIAXNOUME TO XML_EKSODOU
                XmlDeclaration xmldeclaration = null;
                xmldeclaration = doc_out.CreateXmlDeclaration("1.0", "utf-8", null);
                doc_out.InsertBefore(xmldeclaration, doc_out.DocumentElement);
                XmlAttribute attr1 = doc_out.CreateAttribute("xmlns:"+prefix);

                attr1.Value = NamspaceURI;
                XmlElement root_element = null;
                root_element = doc_out.CreateElement(prefix, "EntityDetrimentalData", NamspaceURI);
                doc_out.InsertAfter(root_element, xmldeclaration);                
                doc_out.DocumentElement.Attributes.Append(attr1);
               

                //VRISKOUME TO PELATH POU KANEI TO ERWTHMA
                SqlCommand cmd = null;
                cmd = new SqlCommand("SELECT_CUSTOMER", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CUSTOMER_USERNAME", username));
                cmd.Parameters.Add(new SqlParameter("@CUSTOMER_PASSWORD", password));
                SqlParameter ID_PARAM = new SqlParameter("@CUSTOMER_ID", 0);
                ID_PARAM.Direction = ParameterDirection.Output;
                ID_PARAM.SqlDbType = SqlDbType.Int;
                cmd.Parameters.Add(ID_PARAM);
                int CUSTOMER_ID = 0;

                con.Open();
                cmd.ExecuteNonQuery();
                CUSTOMER_ID = (int)ID_PARAM.Value;
                con.Close();

                //AN VRETHEI O PELATHS DIAVAZOUME TA STOIXEIA TOU ERWTHMATOS

                if (CUSTOMER_ID != 0)
                {
                    XmlNodeList list = doc.SelectNodes("//XML/PERSONCOMPANIES");
                    string output = "";

                    foreach (XmlNode NODE in list)
                    {
                        con.Open();
                        XmlDocument doc1 = new XmlDocument();
                        doc1.LoadXml(NODE.InnerXml);
                        int QUESTION_ID = 0;
                        string personcompanyid = doc1.SelectSingleNode("PERSONCOMPANY/PERSONCOMPANYID").InnerXml;

                        string lastname = doc1.SelectSingleNode("PERSONCOMPANY/LASTNAME").InnerXml;
                        string firstname = doc1.SelectSingleNode("PERSONCOMPANY/FIRSTNAME").InnerXml;
                        string fathername = doc1.SelectSingleNode("PERSONCOMPANY/FATHERNAME").InnerXml;
                        string companyname = doc1.SelectSingleNode("PERSONCOMPANY/COMPANYNAME").InnerXml;
                        string IDCARD = "";
                        if ((doc1.SelectSingleNode("PERSONCOMPANY/IDCARD")) is XmlNode)
                        {
                            IDCARD = doc1.SelectSingleNode("PERSONCOMPANY/IDCARD").InnerXml;
                        }
                        string TAXNUMBER = doc1.SelectSingleNode("PERSONCOMPANY/TAXNUMBER").InnerXml;
                        if ((doc1.SelectSingleNode("PERSONCOMPANY/IDCARD")) is XmlNode)
                        {
                            IDCARD = doc1.SelectSingleNode("PERSONCOMPANY/IDCARD").InnerXml;
                        }
                        string address = doc1.SelectSingleNode("PERSONCOMPANY/ADDRESS").InnerXml;
                        string CITY = doc1.SelectSingleNode("PERSONCOMPANY/CITY").InnerXml;
                        string COUNTY = doc1.SelectSingleNode("PERSONCOMPANY/COUNTY").InnerXml;
                        string PHONE = doc1.SelectSingleNode("PERSONCOMPANY/PHONE").InnerXml;
                        string POSTCODE = "";
                        if ((doc1.SelectSingleNode("PERSONCOMPANY/POSTCODE")) is XmlNode)
                        {
                            POSTCODE = doc1.SelectSingleNode("PERSONCOMPANY/POSTCODE").InnerXml;
                        }

                        //EISAGOUME TO ERWTHMA
                        cmd = new SqlCommand("INSERT_QUESTION", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter ID_PARAM_QUESTION = new SqlParameter("@QUESTION_ID", 0);
                        ID_PARAM_QUESTION.Direction = ParameterDirection.Output;
                        ID_PARAM_QUESTION.SqlDbType = SqlDbType.Int;
                        cmd.Parameters.Add(ID_PARAM_QUESTION);
                        if (!string.IsNullOrEmpty(personcompanyid))
                        {
                            cmd.Parameters.Add(new SqlParameter("@VODAFONE_CODE", personcompanyid));
                        }
                        if (!string.IsNullOrEmpty(lastname))
                        {
                            cmd.Parameters.Add(new SqlParameter("@LASTNAME", lastname));
                        }
                        if (!string.IsNullOrEmpty(firstname))
                        {
                            cmd.Parameters.Add(new SqlParameter("@FIRSTNAME", firstname));
                        }
                        if (!string.IsNullOrEmpty(fathername))
                        {
                            cmd.Parameters.Add(new SqlParameter("@FATHERNAME", fathername));
                        }
                        if (!string.IsNullOrEmpty(companyname))
                        {
                            cmd.Parameters.Add(new SqlParameter("@COMPANYNAME", companyname));
                        }
                        if (!string.IsNullOrEmpty(IDCARD))
                        {
                            cmd.Parameters.Add(new SqlParameter("@IDCARD", IDCARD));
                        }
                        if (!string.IsNullOrEmpty(TAXNUMBER))
                        {
                            cmd.Parameters.Add(new SqlParameter("@TAXNUMBER", TAXNUMBER));
                        }
                        if (!string.IsNullOrEmpty(address))
                        {
                            cmd.Parameters.Add(new SqlParameter("@ADDRESS", address));
                        }
                        if (!string.IsNullOrEmpty(CITY))
                        {
                            cmd.Parameters.Add(new SqlParameter("@CITY", CITY));
                        }
                        if (!string.IsNullOrEmpty(COUNTY))
                        {
                            cmd.Parameters.Add(new SqlParameter("@COUNTY", COUNTY));
                        }
                        if (!string.IsNullOrEmpty(PHONE))
                        {
                            cmd.Parameters.Add(new SqlParameter("@PHONE", PHONE));
                        }
                        if (!string.IsNullOrEmpty(POSTCODE))
                        {
                            cmd.Parameters.Add(new SqlParameter("@POSTCODE", POSTCODE));
                        }
                        cmd.Parameters.Add(new SqlParameter("@CUSTOMER_ID", CUSTOMER_ID));
                        cmd.Parameters.Add(new SqlParameter("@INPUT", sInput));


                        cmd.ExecuteNonQuery();
                        QUESTION_ID = (int) ID_PARAM_QUESTION.Value;

                        con.Close();

                        //PROETOIMASIA APANTHSHS
                      
                        XmlElement xmlpr_cmpname = null;
                        xmlpr_cmpname = doc_out.CreateElement(prefix, "entityName", NamspaceURI);
                        XmlElement xmlpr_cmptype = null;
                        xmlpr_cmptype = doc_out.CreateElement(prefix, "entityType", NamspaceURI);
                        XmlElement xmlpr_vat = null;
                        xmlpr_vat = doc_out.CreateElement(prefix,"vatNumber",NamspaceURI);

                        //PSAXNOUME STO INFO
                        con_trust.Open();
                        //Vriskoume ta Details tou Company
                        cmd = new SqlCommand("SELECT TOP 1  PRS_CMP_ID, CASE WHEN PRS_CMP_IS_COMPANY = 1 THEN 'COMPANY' ELSE 'INDIVIDUAL' END AS PRS_CMP_TYPE, (PRS_CMP_LASTNAME +' ' + PRS_CMP_FIRSTNAME )AS PRS_CMP_FULLNAME, PRS_CMP_COMPANY_NAME FROM    PERSON_COMPANY                     WHERE   PRS_CMP_AFM = '" + TAXNUMBER + "' AND PRS_CMP_ACTIVE = 1 AND PRS_CMP_NOVIEW = 0 AND PRS_CMP_RATED = 1 ", con_trust);
                        cmd.CommandType = CommandType.Text;
                        SqlDataReader read_rd;
                        output = "False";
                        read_rd = cmd.ExecuteReader();
                        if (read_rd.HasRows)
                        {
                            while (read_rd.Read())
                            {
                                xmlpr_cmpname.InnerText = read_rd["PRS_CMP_FULLNAME"].ToString() + read_rd["PRS_CMP_COMPANY_NAME"].ToString();
                                xmlpr_cmptype.InnerText = read_rd["PRS_CMP_TYPE"].ToString();
                                xmlpr_vat.InnerText = TAXNUMBER;
                            }
                        }
                        read_rd.Close();

                        root_element.AppendChild(xmlpr_cmptype);                   
                        root_element.AppendChild(xmlpr_vat);
                        root_element.AppendChild(xmlpr_cmpname);

                        cmd = new SqlCommand("REALCHECK_CMAPPL_VODAFONE3", con_trust);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader READER = null;
                        cmd.Parameters.Add(new SqlParameter("@PRS_CMP_AFM", TAXNUMBER));
                        

                        READER = cmd.ExecuteReader();
                        while (READER.Read())
                        {
                            output = "True";
                            XmlElement xmlresponse = null;
                            xmlresponse = doc_out.CreateElement(prefix,"EntityDetrimentalRecord",NamspaceURI);

                            XmlElement detrimentalType = null;
                            detrimentalType = doc_out.CreateElement(prefix,"detrimentalType",NamspaceURI);
                            detrimentalType.InnerText = READER["TRCS_INFO_CATEGORY"].ToString();
                            xmlresponse.AppendChild(detrimentalType);

                            XmlElement detrimentalYear = null;
                            detrimentalYear = doc_out.CreateElement(prefix,"detrimentalYear",NamspaceURI);
                            detrimentalYear.InnerText = READER["TRCS_INFO_DATE_YEAR"].ToString();
                            xmlresponse.AppendChild(detrimentalYear);

                            XmlElement detrimentalItems = null;
                            detrimentalItems = doc_out.CreateElement(prefix, "detrimentalItems", NamspaceURI);
                            detrimentalItems.InnerText = READER["TRCS_COUNTOF_ACTIVE"].ToString();
                            xmlresponse.AppendChild(detrimentalItems);

                            XmlElement detrimentalValue = null;
                            detrimentalValue = doc_out.CreateElement(prefix, "detrimentalValue", NamspaceURI);
                            detrimentalValue.InnerText = READER["TRCS_SUMOF_ACTIVE"].ToString();
                            xmlresponse.AppendChild(detrimentalValue);

                            XmlElement detrimentalSettledItems = null;
                            detrimentalSettledItems = doc_out.CreateElement(prefix, "detrimentalSettledItems", NamspaceURI);
                            detrimentalSettledItems.InnerText = READER["TRCS_COUNTOF_INACTIVE"].ToString();
                            xmlresponse.AppendChild(detrimentalSettledItems);

                            XmlElement detrimentalSettledValue = null;
                            detrimentalSettledValue = doc_out.CreateElement(prefix, "detrimentalSettledValue", NamspaceURI);
                            detrimentalSettledValue.InnerText = READER["TRCS_SUMOF_INACTIVE"].ToString();
                           
                            xmlresponse.AppendChild(detrimentalSettledValue);

                            root_element.AppendChild(xmlresponse);
                        }
                        READER.Close();
                        con_trust.Close();

                     
                        //    KATAXWROUME THN APANTHSH
                        con.Open();
                        cmd = new SqlCommand("INSERT_ANSWER", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@QUESTION_ID", QUESTION_ID));
                        cmd.Parameters.Add(new SqlParameter("@ANSWER_RESULT", output));
                        cmd.Parameters.Add(new SqlParameter("@ANSWER_XML", root_element.InnerXml));                     

                        cmd.ExecuteNonQuery();
                        con.Close();

                    }


                    //   doc_out.Save("c:\inetpub\wwwroot\vodafonewebservice\test.xml")
                    return doc_out;
                    //AN DEN VRETHEI PELATHS STELNOUME ERROR KAI PERNAME TO ERWTHMA SAN MH APANTHMENO
                }
                else
                {
                    XmlDocument outXML = new XmlDocument();
                    outXML.InnerXml = "<XML><ERROR><MESSAGE>" + "CUSTOMER NOT FOUND, CHECK USERNAME AND PASSWORD" + "</MESSAGE></ERROR></XML>";

                    SaveError("CUSTOMER NOT FOUND, CHECK USERNAME AND PASSWORD", sInput);
                    return outXML;
                }

            }
            catch (Exception ex)
            {
                XmlDocument outXML = new XmlDocument();
                outXML.InnerXml = "<XML><ERROR><MESSAGE>" + ex.Message + "</MESSAGE></ERROR></XML>";

                SaveError(ex.Message, sInput);
                return outXML;

            }
            finally
            {
                con.Close();

            }



        }


        public void SaveError(string StrError, string ErrorInput)
        {
            //Dim doc As XmlDocument
            // doc.InnerXml = ErrorInput

            string con_string = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(con_string);

            SqlCommand cmd = null;

            cmd = new SqlCommand("INSERT_UNANSWERED_QUESTION", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@INPUT", ErrorInput));
            cmd.Parameters.Add(new SqlParameter("@ERROR", StrError));


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

        }
    }
    public class TraceExtension : SoapExtension
    {

        private Stream oldStream;
        private Stream newStream;

        private string m_filename;
        // Save the Stream representing the SOAP request or SOAP response into
        // a local memory buffer.
        public override Stream ChainStream(Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }

        // When the SOAP extension is accessed for the first time, the XML Web
        // service method it is applied to is accessed to store the file
        // name passed in, using the corresponding SoapExtensionAttribute.    
        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return ((TraceExtensionAttribute)attribute).Filename;
        }

        // The SOAP extension was configured to run using a configuration file
        // instead of an attribute applied to a specific XML Web service
        // method.  Return a file name based on the class implementing the Web
        // Service's type.
        public override object GetInitializer(Type WebServiceType)
        {
            // Return a file name to log the trace information to, based on the
            // type.
            return System.Configuration.ConfigurationManager.AppSettings["logsPath"] +"\\" + WebServiceType.FullName + ".log";
        }

        // Receive the file name stored by GetInitializer and store it in a
        // member variable for this specific instance.
        public override void Initialize(object initializer)
        {
            m_filename = Convert.ToString(initializer);
        }

        // If the SoapMessageStage is such that the SoapRequest or SoapResponse
        // is still in the SOAP format to be sent or received over the network,
        // save it out to file.
        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                    WriteOutput(message);
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    WriteInput(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
                default:
                    throw new Exception("invalid stage");
            }
        }

        // Write the SOAP message out to a file.
        public void WriteOutput(SoapMessage message)
        {
            newStream.Position = 0;
            FileStream fs = new FileStream(m_filename, FileMode.Append, FileAccess.Write);
            StreamWriter w = new StreamWriter(fs);
            w.WriteLine("-----Response at " + DateTime.Now.ToString());
            w.Flush();
            Copy(newStream, fs);
            w.Close();
            newStream.Position = 0;
            Copy(newStream, oldStream);
        }

        // Write the SOAP message out to a file.
        public void WriteInput(SoapMessage message)
        {
            Copy(oldStream, newStream);
            FileStream fs = new FileStream(m_filename, FileMode.Append, FileAccess.Write);
            StreamWriter w = new StreamWriter(fs);

            w.WriteLine("----- Request at " + DateTime.Now.ToString());
            w.Flush();
            newStream.Position = 0;
            Copy(newStream, fs);
            w.Close();
            newStream.Position = 0;
        }

        public void Copy(Stream fromStream, Stream toStream)
        {
            StreamReader reader = new StreamReader(fromStream);
            StreamWriter writer = new StreamWriter(toStream);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
    }

    // Create a SoapExtensionAttribute for our SOAP Extension that can be
    // applied to an XML Web service method.
    [AttributeUsage(AttributeTargets.Method)]
    public class TraceExtensionAttribute : SoapExtensionAttribute
    {

        private string m_filename = System.Configuration.ConfigurationManager.AppSettings["logsPath"] + "\\log.txt";

        private int m_priority;
        public override Type ExtensionType
        {
            get { return typeof(TraceExtension); }
        }

        public override int Priority
        {
            get { return m_priority; }
            set { m_priority = value; }
        }

        public string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }
    }
    public class ServiceAuthHeader : SoapHeader
    {



        public string Username;

        public string Password;
    }
    public class ServiceAuthHeaderValidation
    {

        public static bool Validate(ServiceAuthHeader soapHeader)
        {



            if (soapHeader == null)
            {


                throw new NullReferenceException("No soap header was specified.");
            }


            if (soapHeader.Username == null)
            {


                throw new NullReferenceException("Username was not supplied for authentication in SoapHeader.");
            }


            if (soapHeader.Password == null)
            {


                throw new NullReferenceException("Password was not supplied for authentication in SoapHeader.");
            }




            if (soapHeader.Username != System.Configuration.ConfigurationManager.AppSettings["username"] || soapHeader.Password != System.Configuration.ConfigurationManager.AppSettings["password"])
            {


                throw new Exception("Please pass the proper username and password for this service.");
            }
            return true;

        }
    }
}