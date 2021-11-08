package crc644fc0d25276617888;


public class DymeBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Dyme.Services.DymeBinder, DymeAndroidService", DymeBinder.class, __md_methods);
	}


	public DymeBinder ()
	{
		super ();
		if (getClass () == DymeBinder.class)
			mono.android.TypeManager.Activate ("Dyme.Services.DymeBinder, DymeAndroidService", "", this, new java.lang.Object[] {  });
	}


	public DymeBinder (java.lang.String p0)
	{
		super (p0);
		if (getClass () == DymeBinder.class)
			mono.android.TypeManager.Activate ("Dyme.Services.DymeBinder, DymeAndroidService", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}

	public DymeBinder (com.dyme.ServiceCore p0)
	{
		super ();
		if (getClass () == DymeBinder.class)
			mono.android.TypeManager.Activate ("Dyme.Services.DymeBinder, DymeAndroidService", "Dyme.Services.SimpleServiceCore, DymeAndroidService", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
