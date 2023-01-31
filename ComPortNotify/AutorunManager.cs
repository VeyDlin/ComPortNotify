using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortNotify;
public static class AutorunManager {
    private static string AppName;
    private static string RunKey;
    private static string AppPath;





    static AutorunManager() {
        AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "ComPortNotify";
        AppPath = Application.ExecutablePath;
        RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    }





    public static bool IsAutorun() {
        RegistryKey? registryKey;

        try {
            registryKey = Registry.LocalMachine.OpenSubKey(RunKey, true);

            if (registryKey is not null) {
                var value = registryKey.GetValue(AppName);

                if (value is null) {
                    return false;
                } 
                
                if (!value.ToString().ToLower().Equals(AppPath.ToLower())) {
                    return false;
                }

                return true;
            }
        } catch { }


        try {
            registryKey = Registry.CurrentUser.OpenSubKey(RunKey, true);
            var value = registryKey.GetValue(AppName);

            if (value is null) {
                return false;
            }

            if (!value.ToString().ToLower().Equals(AppPath.ToLower())) {
                return false;
            }

            return true;
        } catch { }

        return false;

    }





    public static bool SetAutorun() {
        RegistryKey? registryKey;

        try {
            registryKey = Registry.LocalMachine.OpenSubKey(RunKey, true);

            if (registryKey is not null) {
                registryKey.SetValue(AppName, AppPath);
                return true;
            }
        } catch { }


        try {
            registryKey = Registry.CurrentUser.OpenSubKey(RunKey, true);

            if (registryKey is not null) {
                registryKey.SetValue(AppName, AppPath);
                return true;
            }

        } catch { }

        return false;
    }





    public static bool RemoveAutorun() {
        RegistryKey? registryKey;

        try {
            registryKey = Registry.LocalMachine.OpenSubKey(RunKey, true);

            if (registryKey is not null) {
                if (registryKey.GetValue(AppName).ToString().ToLower() == AppPath.ToLower()) {
                    registryKey.DeleteValue(AppName);
                    return true;
                }
            }  
        } catch { }


        try {
            registryKey = Registry.CurrentUser.OpenSubKey(RunKey, true);

            if (registryKey is not null) {
                if (registryKey.GetValue(AppName).ToString().ToLower() == AppPath.ToLower()) {
                    registryKey.DeleteValue(AppName);
                    return true;
                }
            }
        } catch { }

        return false;
    }
}
