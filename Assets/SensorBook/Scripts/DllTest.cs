using System.Runtime.InteropServices;

public class DllTest
{

	[DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
	public static extern bool GetOpenFileName([In][Out] FileDialogMsg ofn);

	[DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
	public static extern bool GetSaveFileName([In][Out] SaveFileDlg ofn);

	public static bool GetOpenFileName1([In][Out] FileDialogMsg ofn)
	{
		return GetOpenFileName(ofn);
	}

	public static bool GetSaveFileName1([In][Out] SaveFileDlg ofn)
    {
		return GetSaveFileName(ofn);
	}
}
