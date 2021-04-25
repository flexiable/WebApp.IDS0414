using System;
using Novell.Directory.Ldap;

namespace WebApi.IDP0415
{
    /// <summary>
    ///  LDAP工具类
    /// </summary>
    public static class LDAPUtil
    {
        public static string Domain = "apac";//域名称
        public static string Host = "apac.contoso.com";//域服务器地址
        public static string BaseDC = "DC=apac,DC=contoso,DC=com";//根据上面的域服务器地址，每个点拆分为一个DC，例如上面的apac.contoso.com，拆分后就是DC=apac,DC=contoso,DC=com
        public static int Port = 389;//域服务器端口，一般默认就是389
        public static string DomainAdminUser = "admin";//域管理员账号用户名，如果只是验证登录用户，不对域做修改，可以就是登录用户名
        public static string DomainAdminPassword = "1qaz!QAZ";//域管理员账号密码，如果只是验证登录用户，不对域做修改，可以就是登录用户的密码

        /// <summary>
        /// 验证域用户的账号和密码
        /// </summary>
        /// <param name="username">域用户的账号</param>
        /// <param name="password">域用户的密码</param>
        /// <returns>true验证成功，false验证失败</returns>
        public static bool Validate(string username, string password)
        {
            try
            {
                using (var conn = new LdapConnection())
                {
                    conn.Connect(Host, Port);
                    conn.Bind(Domain + "\\" + DomainAdminUser, DomainAdminPassword);//这里用户名或密码错误会抛出异常LdapException

                    var entities =
                        conn.Search(BaseDC, LdapConnection.ScopeSub,
                            $"sAMAccountName={username}",//注意一个多的空格都不能打，否则查不出来
                            new string[] { "sAMAccountName", "cn", "mail" }, false);

                    string userDn = null;
                    while (entities.HasMore())
                    {
                        var entity = entities.Next();
                        var sAMAccountName = entity.GetAttribute("sAMAccountName")?.StringValue;
                        var cn = entity.GetAttribute("cn")?.StringValue;
                        var mail = entity.GetAttribute("mail")?.StringValue;

                        Console.WriteLine($"User name : {sAMAccountName}");//james
                        Console.WriteLine($"User full name : {cn}");//James, Clark [james]
                        Console.WriteLine($"User mail address : {mail}");//james@contoso.com

                        //If you need to Case insensitive, please modify the below code.
                        if (sAMAccountName != null && sAMAccountName == username)
                        {
                            userDn = entity.Dn;
                            break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(userDn)) return false;
                    conn.Bind(userDn, password);//这里用户名或密码错误会抛出异常LdapException
                    // LdapAttribute passwordAttr = new LdapAttribute("userPassword", password);
                    // var compareResult = conn.Compare(userDn, passwordAttr);
                    conn.Disconnect();
                    return true;
                }
            }
            catch (LdapException ldapEx)
            {
                string message = ldapEx.Message;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}