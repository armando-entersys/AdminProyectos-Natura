function AppViewModel() {
    var self = this;

    self.firstName = ko.observable("John");
    self.lastName = ko.observable("Doe");

    self.fullName = ko.pureComputed(function () {
        return self.firstName() + " " + self.lastName();
    }, self);
}

// Activar Knockout.js
ko.applyBindings(new AppViewModel());