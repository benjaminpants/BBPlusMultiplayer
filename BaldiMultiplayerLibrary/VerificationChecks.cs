using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BaldiNetworking
{
	public static class VerificationChecks
	{
		public static int CurrentID = 0;
		public static string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		//Underscore and numbers are also a valid characters but there should only be ONCE per username and thats for duplicate usernames
		public static bool CheckForUsernameChar(this char character)
		{
			for (int i = 0; i < ValidCharacters.Length; i++)
			{
				if (ValidCharacters[i] == character)
				{
					return true;
				}
			}
			return false;
		}

		public static string VerifyUsername(string Name, List<string> ExistingNames, bool ValidateCharacters = true)
		{
			Name = Name.Trim();
			if (ValidateCharacters)
			{
				for (int i = 0; i < Name.Length; i++)
				{
					if (Name[i].CheckForUsernameChar() == false)
					{
						Name = Name.Replace(Name[i], 'A'); //TODO: Make this error, not just silently replace the character with an A
					}
				}
			}
			for (int i = 0; i < ExistingNames.Count; i++)
			{
				string cur_name = ExistingNames[i];
				if (cur_name == Name)
				{
					CurrentID++;
					Name = Name + "_" + CurrentID;
				}
			}
			return Name;
		}
	}
}
