var tabletisingCommandModule = angular.module("TabletisingCommandService", []);


tabletisingCommandModule.service('tabletisingCommandService', [
        '$http',
        function($http) {
            /* This function is for loading list of kneading commands from server. Return a promise project for client-handling actions.*/
            this.loadKneadingCommandsList = function (conditions) {
                return $http.post('/TabletisingCommandSubSystem/CreateTabletisingCommand/SearchKneadingCommands', conditions, null);
            }

            /**
             * This function is for deleting a kneading command base on specific conditions.
             * @param {} kneadingCommand 
             * @returns {} 
             */
            this.deleteKneadingCommand = function(kneadingCommand) {
                return $http.post('/TabletisingCommandSubSystem/CreateTabletisingCommand/DeleteKneadingCommand', kneadingCommand, null);
            }

            /**
             * Base on kneading command to load product information.
             * @param {} kneadingCommand 
             * @returns {} 
             */
            this.loadProductInformation = function(conditions) {
                return $http.post('/TabletisingCommandSubSystem/CreateTabletisingCommand/SearchProductInformation', conditions, null);
            }

            /**
             * Load product details.
             * @param {} conditions 
             * @returns {} 
             */
            this.loadProductDetails = function(conditions) {
                return $http.post('/TabletisingCommandSubSystem/CreateTabletisingCommand/SearchProductDetails', conditions, null);
            }

            /**
             * Tabletize command
             * @param {} conditions 
             * @returns {} 
             */
            this.tabletizeCommand = function (conditions) {
                return $http.post('/TabletisingCommandSubSystem/CreateTabletisingCommand/Go', conditions, null);
            }

            /**
             * Update tabletising command.
             * @param {} conditions 
             * @returns {} 
             */
            this.updateTabletizingCommand = function(conditions) {
                return $http.post('/TabletisingCommandSubSystem/CreateTabletisingCommand/Update', conditions, null);
            }
        }
]);