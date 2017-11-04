angular.module("ManagementReportConsumerService", [])
    .service("managementReportConsumerTable", ['$http',
        function ($http) {


        	this.loadTableRecordsAsync = function (yearmonth,page) {

        		var data = JSON.stringify({
        		    yearmonth: yearmonth,
                    page:page
        		});
        		return $http.post('/ManagementReport/MaterialReceivedConsumedList/RetrieveTableRecords', data, null);
        	}
            // calculate item

        	this.CalculateTableRecordsAsync = function (yearmonth,matcode) {

        	    var data = JSON.stringify({
        	        yearmonth: yearmonth,
                    matcode:matcode
        	    });
        	    return $http.post('/ManagementReport/MaterialReceivedConsumedList/CalculateTableRecords', data, null);
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