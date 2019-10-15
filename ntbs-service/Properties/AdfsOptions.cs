public class AdfsOptions
{
    /** This allows us to use group names agnostic of any naming convention prefixes that exist in the setup */
    public string AdGroupsPrefix { get; set; }
    public string NationalTeamAdGroup { get; set; }
    
    /** TB service groups are of the format Global.NIS.NTBS.Service_<service-specifc-postfix> */
    public string ServiceGroupAdPrefix { get; set; }

    /** Group that contains all users of NTBS, used for authorizing access to the app */
    public string BaseUserGroup { get; set; }

    /** Only used in development mode, allows developers to set their group membership via properties */
    public string DevGroup { get; set; }

    
    /** Base url for the AD FS instace */
    public string AdfsUrl { get; set; }
    /** The ntbs url, used as identifier of the app to adfs */
    public string Wtrealm { get; set; }
}