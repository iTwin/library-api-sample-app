/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.Threading.Tasks;

namespace ItwinLibrarySampleApp
    {
    class Program
        {
        static async Task Main (string[] args)          
            {         
            DisplayMainIndex();

            // Retrieve the token using the TryIt button. https://developer.bentley.com/apis/library/operations/create-catalog/
            Console.WriteLine("\n\nCopy and paste the Authorization header from the 'Try It' sample in the APIM front-end:  ");
            string authorizationHeader = Console.ReadLine();
            Console.Clear();

            DisplayMainIndex();

            await using var libraryMgr = new LibraryManager(authorizationHeader);

            // Execute upload component workflow. This will upload a component to catalog
            await libraryMgr.UploadComponentToCatalogWorkflow();

            // Execute create component workflow. This will create a component and related documents/variations/weblinks
            await libraryMgr.CreateComponentWorkflow(true);

            }

        #region Private Methods
        private static void DisplayMainIndex()
            {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("*****************************************************************************************");
            Console.WriteLine("*           iTwin Platform Library Sample App                                                   *");
            Console.WriteLine("*****************************************************************************************\n");
            }

        #endregion
        }
    }
