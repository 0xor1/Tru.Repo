using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tru.Repo.Exceptions;
using Tru.Repo.Scripts;

namespace Tru.Repo
{
    abstract public class Repo: IRepo
    {
        private const string RepoName = "Tru.Repo";
        private static bool HasPatched;

        private string CurrentRepoBeingPatched;
        private int MaxPatchVersion;
        private int CurrentPatchVersion;

        public string ConnectionString { get; private set; }

        public Repo(String connectionString)
        {
            ConnectionString = connectionString;
            PatchRepo();
        }

        /// <summary>
        /// The patch sequence for the current repo.
        /// </summary>
        /// <param name="repoName">Must be a unique repo name within the current system and no more than 100 characters long.</param>
        protected void PatchSequence(String repoName, Action patchSequence)
        {
            if (string.IsNullOrWhiteSpace(repoName))
            {
                throw new InvalidRepoNameError();
            }

            if (CurrentRepoBeingPatched != null)
            {
                throw new NestedPatchSequenceError(CurrentRepoBeingPatched, repoName);
            }

            CurrentRepoBeingPatched = repoName;

            MaxPatchVersion = GetMaxPatchVersion();
            CurrentPatchVersion = 0;

            patchSequence();

            CurrentRepoBeingPatched = null;
        }

        /// <summary>
        /// All database patches should be executed by submitting the patch command text through the Patch method.
        /// The calls to Patch should always occur in the same order, meaning new patches are only ever added on to the end of the 
        /// existing PatchSequence. Patches should only ever change database objects directly related to the Repo being patched.
        /// Patch can only be called inside the second parameter of PatchSequence. Patches should not return or select any values.
        /// </summary>
        /// <param name="patchText">The command text to patch the database.</param>
        protected void Patch(string patchText)
        {
            Patch(new SqlCommand(patchText));
        }

        /// <summary>
        /// All database patches should be executed by submitting the patch SqlCommand through the Patch method.
        /// The calls to Patch should always occur in the same order, meaning new patches are only ever added on to the end of the 
        /// existing PatchSequence. Patches should only ever change database objects directly related to the Repo being patched.
        /// Patch can only be called inside the second parameter of PatchSequence. Patches should not return or select any values.
        /// </summary>
        /// <param name="patchCommand">The SqlCommand to patch the database.</param>
        protected void Patch(SqlCommand patchCommand)
        {
            if (CurrentRepoBeingPatched == null)
            {
                throw new PatchCalledOutsidePatchSequenceError(patchCommand);
            }

            if (MaxPatchVersion >= CurrentPatchVersion)
            {
                CurrentPatchVersion++;
                return;
            }

            Execute(patchCommand, () => patchCommand.ExecuteNonQuery());

            RegisterPatch(patchCommand);

            CurrentPatchVersion++;
        }

        /// <summary>
        /// Executes execute passing in a SqlCommand built with the sqlCommandText and an open SqlConnection attached to it.
        /// The execute Func does not need to be concerned about opening/closing/disposing of the SqlConnection or SqlCommand.
        /// </summary>
        /// <param name="sqlCommandText">The command text to create the SqlCommand with.</param>
        /// <param name="execute">A Func that will have the SqlCommand passed to it after an open SqlConnection has been added to it.</param>
        /// <returns>The value returned by execute</returns>
        protected T Execute<T>(string sqlCommandText, Func<SqlCommand, T> execute)
        {
            return Execute<T>(new SqlCommand(sqlCommandText), execute);
        }

        /// <summary>
        /// Executes execute after attaching an open SqlConnection to command.
        /// The execute Func does not need to be concerned about opening/closing/disposing of the SqlConnection or SqlCommand.
        /// </summary>
        /// <param name="command">The SqlCommand to run inside execute</param>
        /// <param name="execute"></param>
        /// <returns>The value returned by execute</returns>
        protected T Execute<T>(SqlCommand command, Func<T> execute)
        {
            return Execute(command, (cmd) => execute());
        }

        /// <summary>
        /// Executes execute passing in the SqlCommand with an open SqlConnection attached to it.
        /// The execute Func does not need to be concerned about opening/closing/disposing of the SqlConnection or SqlCommand.
        /// </summary>
        /// <param name="command">The SqlCommand to run inside execute.</param>
        /// <param name="execute">A Func that will have the SqlCommand passed to it after an open SqlConnection has been added to it.</param>
        /// <returns>The value returned by execute</returns>
        protected T Execute<T>(SqlCommand command, Func<SqlCommand, T> execute)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (command)
            {
                command.Connection = connection;
                connection.Open();
                return execute(command);
            }
        }

        /// <summary>
        /// Iterates over the SqlDataReader, calling forEach on each iteration, after all rows have been
        /// processed the reader is disposed of.
        /// </summary>
        /// <param name="reader">The SqlDataReader to iterate over and dispose afterwards.</param>
        /// <param name="forEach">The Action to take on each row from the SqlDataReader</param>
        /// <returns>The number of iterations performed.</returns>
        protected int ForEach(SqlDataReader reader, Action<SqlDataReader> forEach)
        {
            int count = 0;

            using (reader)
            {
                while (reader.Read())
                {
                    count++;
                    forEach(reader);
                }
            }

            return count;
        }

        private int GetMaxPatchVersion()
        {
            var command = new SqlCommand(RepoScript.GetMaxPatchVersion);

            command.Parameters.Add("@RepoName", SqlDbType.NVarChar, 100).Value = CurrentRepoBeingPatched;

            return (int)Execute(command, () => command.ExecuteScalar());
        }

        private void RegisterPatch(SqlCommand patchCommand)
        {
            var command = new SqlCommand(RepoScript.RegisterPatch);

            command.Parameters.Add("@RepoName", SqlDbType.NVarChar, 100).Value = CurrentRepoBeingPatched;
            command.Parameters.Add("@PatchVersion", SqlDbType.Int).Value = CurrentPatchVersion;
            command.Parameters.Add("@PatchText", SqlDbType.NVarChar, -1).Value = patchCommand.CommandText;

            Execute(command, () => command.ExecuteNonQuery());
        }

        #region Patch
        private void PatchRepo()
        {
            if (!HasPatched)
            {
                PatchSequence(RepoName, () =>
                {
                    Patch(RepoScript.Patch0);
                });
                HasPatched = true;
            }
        }
        #endregion
    }
}
