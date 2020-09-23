
using UnityEngine;

namespace ImageAndVideoPicker
{
	public class PermissionManager
	{
		#if UNITY_ANDROID
		private const string READ_STORAGE_PERMISSION = "android.permission.READ_EXTERNAL_STORAGE";
		private const string WRITE_STORAGE_PERMISSION = "android.permission.WRITE_EXTERNAL_STORAGE";

		public PermissionManager()
		{
			StoragePermissionRequest ();
		}

		bool CheckPermissions()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return true;
			}
			
			return (AndroidPermissionsManager.IsPermissionGranted(READ_STORAGE_PERMISSION) && AndroidPermissionsManager.IsPermissionGranted(WRITE_STORAGE_PERMISSION));
		}
		
		
		void StoragePermissionRequest()
		{
			if (CheckPermissions ())
				return;
			
			AndroidPermissionsManager.RequestPermission(new []{READ_STORAGE_PERMISSION, WRITE_STORAGE_PERMISSION}, new AndroidPermissionCallback(
				grantedPermission =>
				{
				// permission granted.
			},
			deniedPermission =>
			{
				// The permission was denied.
			}));
		}
		#endif
	}

	public class AndroidPicker
	{
		#if UNITY_ANDROID


		static AndroidJavaClass _plugin;

		static AndroidPicker()
		{

			if (Application.platform == RuntimePlatform.Android) {
				_plugin = new AndroidJavaClass ("com.astricstore.imageandvideopicker.AndroidPicker");
			}
		}

		public static void CheckPermissions()
		{
			new PermissionManager ();
		}

		public static void BrowseImage()
		{
			if (Application.platform == RuntimePlatform.Android) {

				_plugin.CallStatic ("BrowseForImage", false, 1, 1);
			}
			
		}

		public static void BrowseImage(bool cropping, int aspectX = 1, int aspectY = 1)
		{
			if (Application.platform == RuntimePlatform.Android) {

				_plugin.CallStatic ("BrowseForImage", cropping, aspectX, aspectY);
			}

		}

		public static void BrowseVideo()
		{
			if (Application.platform == RuntimePlatform.Android) {

				_plugin.CallStatic ("BrowseForVideo");
			}

		}

#endif
	}
}


