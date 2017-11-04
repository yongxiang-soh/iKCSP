remarkAttachmentHelper = (function (window, undefined) {
    var checkContainPersonalTag = function (array, tag) {
        for (var i = 0; i < array.length; i++) {
            if (array[i].Tag === tag) {
                return i;
            }
        }
        return -1;
    }

    var getPersonalTags = function () {
        var resultArray = new Array();
        var personalTagName = suggestionHelper.getData("PersonalTagIds");
        var personalTagArray = $("#remarkAttachmentEditor #PersonalTagIds").data("personalTags");
        var i;
        for (i = 0; i < personalTagName.length; i++) {
            var index = checkContainPersonalTag(personalTagArray, personalTagName[i].Name);
            if (index >= 0) {
                resultArray.push(personalTagArray[index]);
            } else {
                resultArray.push({ Id: "00000000-0000-0000-0000-000000000000", Tag: personalTagName[i].Name });
            }
        }
        return resultArray;
    }

    var getObjectData = function () {
        var result = {};
        result.ObjectId = $("#remarksAndAttachmentsObjectId").val();
        result.Remark = $("#remarkAttachmentEditor #remarkInput").val();
        result.RemarkId = $("#remarkAttachmentEditor #remarkId").val();
        result.RemarkIds = $("#remarkAttachmentEditor #listRemarkId").val().split(";").clean("");
        result.AttachmentIds = $("#remarkAttachmentEditor #listAttachmentId").val().split(";").clean("");
        result.PublicTags = $("#remarkAttachmentEditor #PublicTagIds").tokenInput("get");

        //Get Personal Tags
        result.PersonalTags = getPersonalTags();
        return result;
    };
    var saveRemarkAndAttachment = function (successCallBack, errorCallBack) {
        $.ajax({
            type: "POST",
            url: remarkAttachmentSaveUrl,
            data: getObjectData(),
            success: function () {
                if (successCallBack) {
                    successCallBack();
                }
            },
            error: function () {
                if (errorCallBack) {
                    errorCallBack();
                }
            }
        });
    }

    var savePersonalTags = function () {
        var objId = $("#remarkAttachmentEditor #remarksAndAttachmentsObjectId").val();
        var personalTags = getPersonalTags();
        $.ajax({
            type: "POST",
            url: remarkAttachmentSavePersonalTagsUrl,
            data: {
                objectId: objId,
                personalTags: personalTags
            },
            success: function () {
                toastr.success(notification.Save);
            },
        });
    }

    return {
        getObjectData: getObjectData,
        saveRemarkAndAttachment: saveRemarkAndAttachment,
        savePersonalTags: savePersonalTags
    }
})(window);
