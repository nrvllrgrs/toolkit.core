namespace UnityEngine.InputSystem
{
    /// <summary>
    /// Extension methods for <see cref="InputActionProperty"/>.
    /// </summary>
    public static class InputActionPropertyExt
    {
		/// <summary>
		/// Enable the action held on to by the <paramref name="property"/> regardless of whether
		/// it's a direct action or a reference.
		/// </summary>
		/// <param name="property">The property to operate on.</param>
		public static void Enable(this InputActionProperty property)
		{
			property.action?.Enable();
		}

		/// <summary>
		/// Disable the action held on to by the <paramref name="property"/> regardless of whether
		/// it's a direct action or a reference.
		/// </summary>
		/// <param name="property">The property to operate on.</param>
		public static void Disable(this InputActionProperty property)
		{
			property.action?.Disable();
		}

		/// <summary>
		/// Enable the action held on to by the <paramref name="property"/> only if it represents
		/// an <see cref="InputAction"/> directly. In other words, function will do nothing if the action
		/// has a non-<see langword="null"/> <see cref="InputActionProperty.reference"/> property.
		/// </summary>
		/// <param name="property">The property to operate on.</param>
		/// <remarks>
		/// This can make it easier to allow the enabled state of the <see cref="InputAction"/> serialized with
		/// a <see cref="MonoBehaviour"/> to be owned by the behavior itself, but let a reference type be managed
		/// elsewhere.
		/// </remarks>
		public static void EnableDirectAction(this InputActionProperty property)
        {
            if (property.reference != null)
                return;

            property.Enable();
        }

        /// <summary>
        /// Disable the action held on to by the <paramref name="property"/> only if it represents
        /// an <see cref="InputAction"/> directly. In other words, function will do nothing if the action
        /// has a non-<see langword="null"/> <see cref="InputActionProperty.reference"/> property.
        /// </summary>
        /// <param name="property">The property to operate on.</param>
        /// <remarks>
        /// This can make it easier to allow the enabled state of the <see cref="InputAction"/> serialized with
        /// a <see cref="MonoBehaviour"/> to be owned by the behavior itself, but let a reference type be managed
        /// elsewhere.
        /// </remarks>
        public static void DisableDirectAction(this InputActionProperty property)
        {
            if (property.reference != null)
                return;

            property.Disable();
        }

        public static T ReadValue<T>(this InputActionProperty property)
			where T : struct
		{
            return property.action.ReadValue<T>();
        }
    }
}
