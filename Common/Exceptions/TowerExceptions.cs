namespace Common.Exceptions
{
    /// <summary>
    /// Excepción que se lanza cuando no se encuentra ninguna torre en la base de datos,
    /// por ejemplo, al consultar una lista que resulta vacía.
    /// </summary>
    public class TowersNotFoundException : NotFoundException
    {
        public TowersNotFoundException()
            : base("Tower", "any") // Llama al constructor base con un mensaje genérico
        {
            // Puedes sobreescribir el mensaje si lo deseas
            // Message = "No se encontraron torres en el sistema.";
        }
    }

    /// <summary>
    /// Excepción que se lanza al intentar crear una torre con un nombre que ya existe.
    /// </summary>
    public class TowerAlreadyExistsException : ConflictException
    {
        public TowerAlreadyExistsException(string name)
            : base($"Ya existe una torre con el nombre '{name}'.") { }
    }

    /// <summary>
    /// Excepción que se lanza al intentar eliminar una torre que aún tiene entidades asociadas (ej. Apartamentos).
    /// </summary>
    public class TowerInUseException : ConflictException
    {
        public TowerInUseException(int towerId)
            : base($"La torre con ID '{towerId}' no puede ser eliminada porque está en uso.") { }
    }
}