var ValidationModule = (function () {
    // Configuración global para Knockout Validation
    ko.validation.init({
        insertMessages: true,   // Inserta mensajes de error en el DOM
        decorateElement: true,  // Aplica clases CSS en elementos con error
        errorElementClass: 'is-invalid',  // Clase Bootstrap para errores
        errorMessageClass: 'text-danger'  // Clase Bootstrap para mensajes de error
    });

    // Reglas de validación comunes
    var validations = {
        requiredField: function () {
            return ko.observable().extend({ required: { message: "Este campo es obligatorio" } });
        },
        maxLengthField: function (maxLength) {
            return ko.observable().extend({
                required: { message: "Este campo es obligatorio" },
                maxLength: { params: maxLength, message: "El campo debe tener como máximo " + maxLength + " caracteres" }
            });
        },
        emailField: function () {
            return ko.observable().extend({
                required: { message: "El correo electrónico es obligatorio" },
                email: { message: "Ingresa un formato válido de correo electrónico" }
            });
        },
        dateField: function () {
            return ko.observable().extend({
                required: { message: "La fecha es obligatoria" },
                date: { message: "Ingresa una fecha válida" }
            });
        }
        // Puedes agregar más validaciones personalizadas aquí
    };

    return {
        validations: validations
    };
})();