console.log(typeof ko);
function Task(id, title) {
    this.id = id;
    this.title = ko.observable(title);
}

function Column(id, name, tasks) {
    this.id = id;
    this.name = ko.observable(name);
    this.tasks = ko.observableArray(tasks);
}

function AppViewModel() {
    var self = this;

    self.columns = ko.observableArray([
        new Column(1, 'To Do', [new Task(1, 'Task 1'), new Task(2, 'Task 2')]),
        new Column(2, 'In Progress', [new Task(3, 'Task 3')]),
        new Column(3, 'Done', [new Task(4, 'Task 4')])
    ]);
    self.columns2 = ko.observableArray();
    self.addTask = function (column) {
        var newId = column.tasks().length + 1;
        column.tasks.push(new Task(newId, 'New Task ' + newId));
    };
    // Método para limpiar las tareas de una columna específica
    self.clearTasks = function (column) {
        column.tasks.removeAll(); // Limpia todas las tareas de la columna
    };
    self.refreshTasks = function (column) {
        // Guardamos las tareas actuales
        var currentTasks = column.tasks().slice(0);

        // Limpiamos el observableArray
        column.tasks.removeAll();

        // Vuelve a añadir las tareas después de un pequeño retraso (simulando una recarga)
            column.tasks(currentTasks);
        // Forzar la actualización manualmente
        column.tasks.valueHasMutated();  // Esto hace que Knockout redibuje la vista
    };
    self.refreshTasks = function (column, taskToMove, targetColumn) {
        // Guardamos las tareas actuales de ambas columnas
        var currentTasksSource = column.tasks().slice(0);  // Columna origen
        var currentTasksTarget = targetColumn.tasks().slice(0);  // Columna destino

        // Limpiamos la tarea de la columna origen
        column.tasks.remove(taskToMove);

        // Añadimos la tarea a la columna destino
        targetColumn.tasks.push(taskToMove);

        // Forzar la actualización manualmente en ambas columnas
        column.tasks.valueHasMutated();  // Actualizar columna origen
        targetColumn.tasks.valueHasMutated();  // Actualizar columna destino
    };
    self.moveTask = function (taskId, fromColumnId, toColumnId, newIndex) {
        var fromColumn = self.columns().find(column => column.id === fromColumnId);
        var toColumn = self.columns().find(column => column.id === toColumnId);

        var task = fromColumn.tasks().find(task => task.id === taskId);
        fromColumn.tasks.remove(task);
        toColumn.tasks.splice(newIndex, 0, task);
        self.refreshTasks(fromColumn, task, toColumn);
    };
}
// Aquí está el código de Knockout que usa 'ko'
var appViewModel = new AppViewModel();
ko.applyBindings(appViewModel);

// Initialize SortableJS después de que Knockout haya sido inicializado
document.querySelectorAll('.sortable').forEach(function (element) {
    new Sortable(element, {
        group: 'kanban',
        animation: 150,
        onEnd: function (evt) {
            var taskId = parseInt(evt.item.getAttribute('data-task-id'));
            var fromColumnId = parseInt(evt.from.id);
            var toColumnId = parseInt(evt.to.id);
            var newIndex = evt.newIndex;

            appViewModel.moveTask(taskId, fromColumnId, toColumnId, newIndex);
        }
    });
});


