namespace ServiceContracts
{
    public interface IPersonsDeleterService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>true:the deletion is successful, otherwise unsuccessful</returns>
        Task<bool> DeletePerson(Guid? personID);

    }
}
