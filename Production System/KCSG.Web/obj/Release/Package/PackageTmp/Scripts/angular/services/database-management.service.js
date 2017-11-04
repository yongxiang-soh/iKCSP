angular.module("MasterDataTableService", [])
    .service("masterDataTable", ['$http',
        function ($http) {


        	// This function is for loading list of tables from server.
        	// Return a promise project for client-handling actions.
        	this.loadTablesListAsync = function () {

        	    return $http.post('/SystemManagement/MasterDatabase/RetrieveTablesList', null, null);
        	}

        	// Serialize data and insert it into SQL Database system by making request to server.
        	this.insertRecordAsync = function (table, rows) {

        	    return $http.post('/SystemManagement/MasterDatabase/InsertTableRecords',
                    JSON.stringify({
                    	table: table,
                    	row: rows
                    }));
        	}

        	// This function is for loading table records by using table name, page and records.
        	// It returns a promise object which is for client handling.
        	this.loadTableRecordsAsync = function (table, page, records) {

        		var data = JSON.stringify({
        			table: table,
        			page: page,
        			records: records
        		});
        		return $http.post('/SystemManagement/MasterDatabase/RetrieveTableRecords', data, null);
        	}

        	// This function is for analyzing item and do update into database.
        	this.updateItemAsync = function (table, original, target) {

        		var data = JSON.stringify({
        			table: table,
        			original: original,
        			target: target
        		});

        		return $http.post('/SystemManagement/MasterDatabase/UpdateTableRecord', data);
        	}

        	// This function is for analyzing data and tell server to delete the analyzed one.
        	// It returns a promise object for client-handling.
        	this.deleteItemAsync = function (table, item) {

	            var data = JSON.stringify({
	                table: table,
	                parameters: item
	            });

	            return $http.post("/SystemManagement/MasterDatabase/DeleteTableRecord", data);
        	}
        }]);