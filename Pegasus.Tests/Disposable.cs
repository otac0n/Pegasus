// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.IO;

    public static class Disposable
    {
        public static IDisposable FromAction(Action action)
        {
            return new ActionDisposable(action);
        }

        public static IDisposable TempFile(string contents, out string path)
        {
            IDisposable disposable = null;
            try
            {
                disposable = TempFile(out path);
                File.WriteAllText(path, contents);

                var temp = disposable;
                disposable = null;
                return temp;
            }
            finally
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public static IDisposable TempFile(out string path)
        {
            string tempFile = null;
            var disposable = FromAction(() =>
            {
                try
                {
                    if (tempFile != null)
                    {
                        File.Delete(tempFile);
                    }
                }
                catch (IOException)
                {
                }
            });

            try
            {
                tempFile = Path.GetTempFileName();
                path = tempFile;

                var temp = disposable;
                disposable = null;
                return temp;
            }
            finally
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public static IDisposable TempGeneratedFile(string fileName)
        {
            return FromAction(() =>
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (IOException)
                {
                }
            });
        }

        private class ActionDisposable : IDisposable
        {
            private readonly Action action;

            public ActionDisposable(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                this.action();
            }
        }
    }
}
