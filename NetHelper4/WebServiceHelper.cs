using System;
using System.Text;
using System.Net;
using System.IO;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Web.Services.Description;

namespace NetHelper4
{
    public static class WebServiceHelper
    {
        public static object InvokeWebService(string url, string methodName, object[] args)
        {
            return InvokeWebService(url, null, methodName, args);
        }

        private static object InvokeWebService(string url, string className, string methodName, object[] args)
        {
            string @namespace = "ServiceBase.WebService.DynamicWebLoad";
            if (string.IsNullOrEmpty(className))
            {
                className = GetClassName(url);
            }
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(string.Format("{0}?WSDL", url));
            ServiceDescription sd = ServiceDescription.Read(stream);
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(@namespace);

            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();
            ICodeCompiler icc = csc.CreateCompiler();

            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");

            CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
            if (cr.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }

            Assembly assembly = cr.CompiledAssembly;
            Type t = assembly.GetType(@namespace + "." + className, true, true);
            object obj = Activator.CreateInstance(t);
            MethodInfo mi = t.GetMethod(methodName);
            return mi.Invoke(obj, args);

        }
        private static string GetClassName(string url)
        {
            string[] parts = url.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
}
