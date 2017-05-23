var lines = [];

function init(ls) {
    lines = ls;
}

function getColumn() {
    for (var i = 0; i < lines.length; i++) {

    }
}




var measures = [];
var popupIsEditing = false;


var MaxNumCol;//max num of column of table
var RowIndexEditing, ColIndexEditing;
var MeasureId, LineId;
var selectedCell;


var flagChooseAnyLine = true;
var lineIndexChooseForShowHideColumn = 0;
var DataSourceGrid, numOfLines, numOfMeasures;
var dateSearch;
var departmentId;

function getDateSearch(date) {
    dateSearch = date;
}

function getDepartmentId(dpmId) {
    departmentId = dpmId;
}

function selectLine(e) {
    if (comboboxLine.value() == 0)//Line combobox choosed any line
    {
        MaxNumCol = $("#grid tr:nth-child(1) td").length;
        $('#grid table').each(function (index, item) {
            var count = 0;
            var grid = $("#grid").data("kendoGrid");
            $('#grid table').find('th').each(function () {
                if (count >= 7) {
                    $('#grid table tr td:nth-child(' + (count + 1) + ')').show();
                    //$('#grid table tr th:nth-child(' + (count + 1) + ')').show();
                    grid.showColumn(count);

                    $('#grid table tr').find('td:nth-child(' + (count + 1) + ')').each(function () {
                        if ($(this).attr("colspan") != "")
                            $(this).attr("colspan", "@Model.Lines.Count");
                        if ($(this).attr("IsHiddenForSpanColumns") == "true")
                            $(this).hide();
                    });
                }
                count++;
            })
        });
        flagChooseAnyLine = true;
        return;
    }

    //Line combobox choosed 1 line of lines
    $('#grid table').each(function (index, item) {
        var count = 0;
        var grid = $("#grid").data("kendoGrid");
        if (DataSourceGrid[0].Lines.length > 0) {
            grid.hideColumn(8);
            var dataSource = $("#grid").data("kendoGrid").dataSource;
            var temp = dataSource.data()[0].Lines[0].Value;
            dataSource.data()[0].Lines[0].set("Value", '1');
            //dataSource.data()[0].Lines[0].set("Value", temp);
        }
        $('#grid table').find('th').each(function () {
            if (count >= 7) {
                var text = $(this).text();
                if (text != comboboxLine.text()) {
                    //must edit colspan before hide if not will fail css
                    $('#grid table tr').find('td:nth-child(' + (count + 1) + ')').each(function () {
                        if ($(this).attr("colspan") != "")
                            $(this).attr("colspan", "1");
                    });

                    $('#grid table tr td:nth-child(' + (count + 1) + ')').hide();
                    //$('#grid table tr th:nth-child(' + (count + 1) + ')').hide();
                    grid.hideColumn(count);
                    setTimeout(function () {
                        $('#grid table tr').find('td:nth-child(8)').each(function () {
                            if ($(this).attr("colspan") != "")
                                $(this).show();
                        });
                    }, 10);
                }
                else {
                    //lineIndexChooseForShowHideColumn = count;
                    $('#grid table tr td:nth-child(' + (count + 1) + ')').show();
                    //$('#grid table tr th:nth-child(' + (count + 1) + ')').show();
                    grid.showColumn(count);
                    $('#grid table tr').find('td:nth-child(' + (count + 1) + ')').each(function () {
                        if ($(this).attr("colspan") != "") {
                            $(this).attr("colspan", "1");
                        }
                        if ($(this).attr("IsHiddenForSpanColumns") == "true")
                            $(this).hide();
                    });
                }

            }
            count++;
        });
    });
    MaxNumCol = $("#grid tr:nth-child(1) td").length;
}


var kendoWindow = $("<div id='window'/>").kendoWindow({
    title: "Cell Remark",
    visible: false,
    width: "400px",
    modal: true,
    viewable: false,
    content: {
        template: $("#windowTemplateCellRemark").html()
    },

    //close: onClose,
    //open: onOpen,

}).data("kendoWindow");

var mpsaWindow = $("<div id='window'>").kendoWindow({
    title: "Update MPSA",
    visible: false,
    width: "600px",
    modal: true,
    viewable: false,
    close: onClose,
    open: onOpen,
    content: {
        template: $("#windowTemplateMpsa").html()
    }
}).data("kendoWindow");

var prWindow = $("<div id='window'>").kendoWindow({
    title: "Update PR",
    visible: false,
    width: "600px",
    modal: true,
    viewable: false,
    close: onClose,
    open: onOpen,
    content: {
        template: $("#windowTemplatePr").html()
    }
}).data("kendoWindow");

$("#grid").on("click", "td", function (e) {
    //1 so truong hop khong tu remove class selected --> failse : solusion: manual remove class selected of old cell
    if (selectedCell)
        selectedCell.removeClass("k-state-selected");
    var element = $(e.target);
    if (element.is("input"))
        element.parent().addClass("k-state-selected");
    else
        element.addClass("k-state-selected");
    selectedCell = element;
    if (element.hasClass("cell-popup") && element.parent().attr('is-editable') == 'true') {
        selectedCell = element.parent();
        ColIndexEditing = $(this).parent().children().index($(this));
        RowIndexEditing = $(this).parent().parent().children().index($(this).parent());
        var measureId = $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(3)').text();
        var lineId = element.parent().attr('LineId');
        mpsaWindow.wrapper.find("#measureId").val(measureId);
        mpsaWindow.wrapper.find("#lineId").val(lineId);
        $.ajax({
            cache: false,
            url: "/DdsMeeting/PomUpdate?date=" + dateSearch + "&measureId=" + measureId + "&lineId=" + lineId,
            type: 'get',
            complete: function (data) {
                var result = jQuery.parseJSON(data.responseText);
                mpsaWindow.wrapper.find("#pom-making").val(result.pomMaking);
                mpsaWindow.wrapper.find("#pom-packing").val(result.pomPacking);
                mpsaWindow.wrapper.find("#pom-planing").val(result.pomPlaning);
                mpsaWindow.wrapper.find("#pom-Making-Remark").val(result.pomMakingRemark);
                mpsaWindow.wrapper.find("#pom-Packing-Remark").val(result.pomPackingRemark);
                mpsaWindow.wrapper.find("#pom-Planing-Remark").val(result.pomPlaningRemark);
                mpsaWindow.wrapper.find("#pom-making").kendoNumericTextBox({ format: "0" });
                mpsaWindow.wrapper.find("#pom-packing").kendoNumericTextBox({ format: "0" });
                mpsaWindow.wrapper.find("#pom-planing").kendoNumericTextBox({ format: "0" });
            }
        });
        var columnTitle = $("#grid table tr:nth-child(1) th:nth-child(" + (ColIndexEditing + 1) + ")").text();
        mpsaWindow.title("Update MPSA_" + columnTitle);
        mpsaWindow.open().center();
    }
    if (element.hasClass("pr-cell-popup") && element.parent().attr('is-editable') == 'true') {
        selectedCell = element.parent();
        ColIndexEditing = $(this).parent().children().index($(this));
        RowIndexEditing = $(this).parent().parent().children().index($(this).parent());
        var measureId = $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(3)').text();
        var lineId = element.parent().attr('LineId');
        prWindow.wrapper.find("#measureId").val(measureId);
        prWindow.wrapper.find("#lineId").val(lineId);
        $.ajax({
            cache: false,
            url: "/DdsMeeting/PrUpdate?date=" + dateSearch + "&measureId=" + measureId + "&lineId=" + lineId,
            type: 'Get',
            complete: function (data) {
                var result = jQuery.parseJSON(data.responseText);
                prWindow.wrapper.find("#pr-lastday").val(result.prLastDay);
                prWindow.wrapper.find("#pr-mtd").val(result.prMtd);
                prWindow.wrapper.find("#pr-lastday-remark").val(result.prLastDayRemark);
                prWindow.wrapper.find("#pr-mtd-remark").val(result.prMtdRemark);
                prWindow.wrapper.find("#pr-lastday").kendoNumericTextBox();
                prWindow.wrapper.find("#pr-mtd").kendoNumericTextBox();
            }
        });
        var columnTitle = $("#grid table tr:nth-child(1) th:nth-child(" + (ColIndexEditing + 1) + ")").text();
        prWindow.title("Update PR_" + columnTitle);
        prWindow.open().center();
    }
    //check click event apply for div.cell-remark
    if (element.hasClass("cell-remark")) {
        ColIndexEditing = $(this).parent().children().index($(this));
        RowIndexEditing = $(this).parent().parent().children().index($(this).parent());
        //get measureId from hiden column
        MeasureId = $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(3)').text();
        LineId = element.attr('LineId');
        $('#cellRemarkResult').val($('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(' + (ColIndexEditing + 1) + ')').children('.cell-remark').attr('title'));
        kendoWindow.open().center();
    }
});

var savePom = function () {
    $.ajax({
        cache: false,
        url: "/DdsMeeting/PomUpdate?date=" + dateSearch,
        data: $("#pom-update").serialize(),
        type: 'post',
        complete: function (data) {
            var jData = jQuery.parseJSON(data.responseText);
            if (jData.updated) {
                var pomrow = jQuery.grep(measures, function (m) { return m.Id == jData.measureId; })[0].Index;
                var pmsarow = jQuery.grep(measures, function (m) { return m.Id == jData.pmsaId; })[0].Index;
                var col = jQuery.grep(lines, function (l) { return l.Id == jData.lineId; })[0].Index;
                var dataSource = $("#grid").data("kendoGrid").dataSource;
                //dataSource.fetch(function () {
                dataSource.data()[pomrow].Lines[col].set("Value", jData.result);
                dataSource.data()[pmsarow].Lines[col].set("Value", jData.pmsaResult);
                //});


                var updatecell = '#grid table tbody > tr:nth-child(' + (pomrow + 1) + ') > td:nth-child(' + (col + 8) + ') > div';
                setColorForCellWhenEditSuccess(pomrow, col + 7, jData.result);
                //$(updatecell).html($(updatecell).html().replace($(updatecell).text(), jData.result));
                $(updatecell).text(jData.result);


                var updatecell2 = '#grid table tbody > tr:nth-child(' + (pmsarow + 1) + ') > td:nth-child(' + (col + 8) + ')';
                setColorForCellWhenEditSuccess(pmsarow, col + 7, jData.pmsaResult);
                //$(updatecell2).html($(updatecell2).html().replace($(updatecell2).text(), jData.pmsaResult));
                $(updatecell2).text(jData.pmsaResult);
            }
        }
    });

    mpsaWindow.close();

    selectedNextCell(selectedCell);
};
var cancelPom = function () {
    mpsaWindow.close();
    $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(' + (ColIndexEditing + 1) + ')').removeClass('k-state-selected');
    $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(' + (ColIndexEditing + 1) + ')').children('.cell-popup').attr('aria-selected', 'false');
};
var savePr = function () {
    $.ajax({
        cache: false,
        url: "/DdsMeeting/PrUpdate?date=" + dateSearch,
        data: $("#pr-update").serialize(),
        type: 'post',
        complete: function (data) {
            var jData = jQuery.parseJSON(data.responseText);
            if (jData.updated) {
                var row = jQuery.grep(measures, function (m) { return m.Id == jData.measureId; })[0].Index;
                var col = jQuery.grep(lines, function (l) { return l.Id == jData.lineId; })[0].Index;
                //$("#grid").data("kendoGrid").dataSource.read();
                var dataSource = $("#grid").data("kendoGrid").dataSource;
                dataSource.data()[row].Lines[col].set("Value", jData.result);
                var updatecell = '#grid table tbody > tr:nth-child(' + (row + 1) + ') > td:nth-child(' + (col + 8) + ') > div';
                setColorForCellWhenEditSuccess(row, col + 7, jData.result);
                $(updatecell).text(jData.result);
                //$(updatecell).html($(updatecell).html().replace($(updatecell).text(), jData.result));
            }
        }
    });
    prWindow.close();
    selectedNextCell(selectedCell);
};
var cancelPr = function () {
    prWindow.close();
    $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(' + (ColIndexEditing + 1) + ')').removeClass('k-state-selected');
    $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(' + (ColIndexEditing + 1) + ')').children('.cell-popup').attr('aria-selected', 'false');
};

$("#yesBtnCellRemarkConfirm").click(function () {
    $.ajax({
        url: "/DdsMeeting/CellRemarkUpdate",
        data: {
            lineId: LineId,
            measureId: MeasureId,
            date: dateSearch,
            remark: $('#cellRemarkResult').val()
        },
        type: "POST",
        dataType: 'application/json',
        complete: function (data) {
            $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(' + (ColIndexEditing + 1) + ')').children('.cell-remark').attr('title', jQuery.parseJSON(data.responseText).remark);
        }
    });
    kendoWindow.close();
});
$("#noBtnCellRemarkConfirm").click(function () {
    kendoWindow.close();
});
function onClose(e) {
    popupIsEditing = false;
}
function onOpen(e) {
    popupIsEditing = true;
}


//format css table
function onDataBound(arg) {
    DataSourceGrid = $("#grid").data("kendoGrid").dataSource.data();
    numOfMeasures = DataSourceGrid.length;
    if (numOfMeasures > 0)
        numOfLines = DataSourceGrid[0].Lines.length;


    //console.log(arg.sender.dataSource.data());
    measures = [];
    var mData = arg.sender.dataSource.data();
    for (var i = 0; i < mData.length; i++) {
        measures.push({ Id: mData[i].MeasureId, Index: i });
    }
    //$('#grid>.k-grid-content>table').each(function (index, item) {
    $('#grid table').each(function (index, item) {
        var dimension_col = 1;
        var key = true;//set color for row
        // First, scan first row of headers for the "Dimensions" column.
        //$('#grid>.k-grid-header>.k-grid-header-wrap>table').find('th').each(function () {
        $('#grid table').find('th').each(function () {
            if ($(this).text() == 'Dms') {

                // first_instance holds the first instance of identical td
                var first_instance = null;

                $(item).find('tr').each(function () {
                    var row = $(this);
                    // find the td of the correct column (determined by the colTitle)
                    var dimension_td = $(this).find('td:nth-child(' + dimension_col + ')');

                    if (first_instance == null) {
                        first_instance = dimension_td;
                        first_instance.css("background-color", 'white');
                    } else if (dimension_td.text() == first_instance.text()) {
                        var count = 0;
                        row.children().each(function () {
                            if (count < 7)
                                $(this).css('background-color', first_instance.css("background-color"));
                            else {
                                if ($(this).attr('is-editable') == "true")
                                    $(this).css('background-color', 'white');
                                //console.log($(this).text());
                            }
                            count++;
                        });
                        // console.log(dimension_td.text(), first_instance.text());
                        // if current td is identical to the previous
                        // then remove the current td
                        //dimension_td.remove();
                        dimension_td.css('display', 'none');
                        //console.log(dimension_td);
                        // increment the rowspan attribute of the first instance
                        first_instance.attr('rowspan', typeof first_instance.attr('rowspan') == "undefined" ? 2 : parseInt(first_instance.attr('rowspan')) + 1);
                    } else {
                        // this cell is different from the last
                        first_instance = dimension_td;
                        first_instance.css("background-color", key ? '#d7ffc1' : 'white');
                        var count = 0;
                        row.children().each(function () {
                            if (count < 7)
                                $(this).css('background-color', first_instance.css("background-color"));
                            else {
                                if ($(this).attr('is-editable') == "true")
                                    $(this).css('background-color', 'white');
                            }
                            count++;
                        });
                        //console.log(first_instance.css("background-color"));
                        key = !key;
                    }
                });

                //format backround color for column dms
                $(item).find('tr').each(function () {
                    $(this).find('td:nth-child(1)').css('background-color', '#eea236');
                });
            }
            dimension_col;
        });

        //set MaxNumCol for focus next cell when press enter
        MaxNumCol = $("#grid tr:nth-child(1) td").length;
    });

    //css for td of table to show cell-remark on top right corner
    $('#grid table').each(function (index, item) {
        var count = 0;
        $('#grid table').find('th').each(function () {
            if (count >= 7) {
                $('#grid table tr td:nth-child(' + (count + 1) + ')').css('position', 'relative');
                $('#grid table tr td:nth-child(' + (count + 1) + ')').css('text-align', 'center');
            }
            count++;
        });
    });

    //show remark when hover cells of line
    $("#grid tr td").hover(function () {
        $(this).children('.cell-remark').css("display", "block");
    }, function () {
        $(this).children('.cell-remark').css("display", "none");
    });


    //set gray for cell not allow edit
    $('#grid table tr').each(function () {
        var index = 0;
        $(this).find('td').each(function () {
            if (index >= 7 && $(this).attr('IsGray') == "true") {
                $(this).css('background-color', 'gray');
                $(this).css('color', 'white');
            }
            //do your stuff, you can use $(this) to get current cell
            index++;
        });
    });

    //set color for cell when loading with condition from target columns
    var rolIdx = 0;
    $('#grid table tr').each(function () {
        var index = 0;
        $(this).find('td').each(function () {
            //if (index >= 7 && ($(this).attr('IsGray') != "true" || $(this).attr('is-editable') == "true")) {
            if (index >= 7 && $(this).text() != "") {
                var target = $(this).parent().children().eq(5).text();
                var result = $(this).text();
                if (                    //not color if result = 0 and measure is MPSA(id = 33)
                        (($('#grid tbody tr:nth-child(' + (rolIdx) + ') td:nth-child(3)').text() == 33 && result != 0) || ($('#grid tbody tr:nth-child(' + (rolIdx) + ') td:nth-child(3)').text() != 33))//(check measure is 33 and result != 0) or (measure != 33)
                        &&
                        (
                            (target.indexOf(">") >= 0 || target.indexOf("<") >= 0 || target.indexOf("=") >= 0)//check if have Operators
                                && (!isNaN(parseFloat(result)) && isFinite(result))// check if it is number and not null
                        )
                   ) {
                    var clause = "parseFloat(" + result + ")" + target;
                    if (eval(clause)) {
                        $(this).css('background-color', 'green');
                        $(this).css('color', 'white');
                    } else {
                        $(this).css('background-color', 'red');
                        $(this).css('color', 'white');
                    }
                } else {
                    $(this).css('background-color', 'white');
                    $(this).css('color', '#787878');
                }
            }
            //do your stuff, you can use $(this) to get current cell
            index++;
        });
        rolIdx++;
    });

    //remove selected border when click different cell
    $('#grid table tr td').focusout(function () {
        $(this).removeClass('k-state-focused');
        $(this).removeClass("k-edit-cell");
        $(this).removeClass("k-dirty-cell");

    });

    //select text when click on cell
    $('#grid table tr td').focusin(function () {
        $(this).children('input').select();
    });

    //myFunc();
}

var isEditing = false;

//focus next cell when press enter
function onDataBinding(e) {
    var timeout = null;
    $("#grid").on("focus", "td", function (e) {
        $("input").on("keydown", function (event) {
            e.preventDefault();
            if (event.keyCode == 13) {
                if (popupIsEditing)
                    return;
                isEditing = true;
                timeout = setTimeout(function () {
                    var curCell = $(e.target).parent();//$("#grid").find(".k-state-selected");;
                    var eCell = $("#grid").find(".k-edit-cell");
                    var nextCell = findNextCell(curCell.parent().index(), curCell.index() - 6);
                    //while (nextCell.attr('is-editable') == "false" || nextCell.css('display') == "none" || nextCell.index() < 0) {
                    //    if (nextCell.index() < 0) {
                    //        nextCell = curCell.parent().next().children().first();
                    //        if (nextCell.index() < 0)
                    //            break;
                    //    }
                    //    else {
                    //        if (nextCell.attr('is-editable') == "false" || nextCell.css('display') == "none") {
                    //            if (nextCell.index() == MaxNumCol - 1) {
                    //                if (nextCell.parent().next().children().length > 0)
                    //                    nextCell = nextCell.parent().next().children().first();
                    //                else {
                    //                    nextCell = nextCell.next();
                    //                    break;
                    //                }
                    //            }
                    //            else {
                    //                nextCell = nextCell.next();
                    //            }
                    //        }
                    //    }
                    //}

                    selectedCell = nextCell;
                    curCell.removeClass("k-state-selected");
                    curCell.removeClass("k-state-focused");
                    curCell.removeClass("k-edit-cell");
                    curCell.removeClass("k-dirty-cell");
                    curCell.removeAttr("data-role");
                    if (nextCell.children('.cell-popup').length > 0)
                        showPopup(nextCell.children('.cell-popup'));
                    if (nextCell.children('.pr-cell-popup').length > 0)
                        showPopup(nextCell.children('.pr-cell-popup'));
                    nextCell.addClass("k-state-selected");
                    nextCell.addClass("k-state-focused");
                    try {
                        $('#grid').data('kendoGrid').closeCell(eCell);
                    } catch (ex) {
                    }
                    $('#grid').data('kendoGrid').select();
                    $('#grid').data('kendoGrid').editCell(nextCell);
                    //select text when press enter
                    nextCell.children('input').select();
                    isEditing = false;
                }, 0);
            }
        });
    });

    $("#grid").on("focusout", "td", function (e) {
        clearTimeout(timeout);
    });
}


function findNextCell(rowIndex, colIndex) {
    var firstTime = true;
    for (var i = rowIndex; i < numOfMeasures; i++)
        if (firstTime) {
            for (var j = colIndex; j < numOfLines; j++)
                if (DataSourceGrid[i].Lines[j].IsReadOnly == false && DataSourceGrid[i].Readonly == false && DataSourceGrid[i].Lines[j].IsHiddenForSpanColumns == false)//&& $('#grid tr:nth-child(' + (i + 1) + ') td:nth-child(' + (j + 8) + ')').css('display') != 'none'
                    return $('#grid tr:nth-child(' + (i + 1) + ') td:nth-child(' + (j + 8) + ')');
            firstTime = false;
        }
        else
            for (var j = 0; j < numOfLines; j++)
                if (DataSourceGrid[i].Lines[j].IsReadOnly == false && DataSourceGrid[i].Readonly == false && DataSourceGrid[i].Lines[j].IsHiddenForSpanColumns == false)
                    return $('#grid tr:nth-child(' + (i + 1) + ') td:nth-child(' + (j + 8) + ')');

    return $('#grid tr:nth-child(' + (numOfMeasures + 1) + ') td:nth-child(' + (numOfLines + 8) + ')');
}

function selectedNextCell(currentCell) {
    var curCell = currentCell;//$("#grid").find(".k-state-selected");;
    var nextCell = findNextCell(curCell.parent().index(), curCell.index() - 6);
    selectedCell = nextCell;
    curCell.removeClass("k-state-selected");
    curCell.removeClass("k-state-focused");
    curCell.removeClass("k-edit-cell");
    curCell.removeClass("k-dirty-cell");
    curCell.removeAttr("data-role");
    if (nextCell.children('.cell-popup').length > 0)
        showPopup(nextCell.children('.cell-popup'));
    if (nextCell.children('.pr-cell-popup').length > 0)
        showPopup(nextCell.children('.pr-cell-popup'));
    nextCell.addClass("k-state-selected");
    nextCell.addClass("k-state-focused");
    try {
        $('#grid').data('kendoGrid').closeCell(eCell);
    } catch (ex) {
    }
    $('#grid').data('kendoGrid').select();
    $('#grid').data('kendoGrid').editCell(nextCell);
    //select text when press enter
    nextCell.children('input').select();
}

function recursive(curCell, nextCell) {
    if (nextCell.index() < 0)
        return recursive(curCell, curCell.parent().next().children().first());
    if (nextCell.attr('is-editable') == "false" || nextCell.css('display') == "none") {
        //if (nextCell.index() == nextCell.siblings().size()) {
        if (nextCell.index() == MaxNumCol - 1) {
            if (nextCell.parent().next().children().length > 0)
                return recursive(curCell, nextCell.parent().next().children().first());
            else
                return nextCell.next();
        }
        else {
            return recursive(curCell, nextCell.next());
        }
    }
    return nextCell;
}



function showPopup(e) {
    var element = e;
    if (element.hasClass("cell-popup")) {
        ColIndexEditing = $(element.parent()).parent().children().index($(element.parent()));
        RowIndexEditing = $(element.parent()).parent().parent().children().index($(element.parent()).parent());
        var measureId = $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(3)').text();
        var lineId = element.parent().attr('LineId');
        mpsaWindow.wrapper.find("#measureId").val(measureId);
        mpsaWindow.wrapper.find("#lineId").val(lineId);
        $.ajax({
            cache: false,
            url: "/DdsMeeting/PomUpdate?date=" + dateSearch + "&measureId=" + measureId + "&lineId=" + lineId,
            type: 'get',
            complete: function (data) {
                var result = jQuery.parseJSON(data.responseText);
                mpsaWindow.wrapper.find("#pom-making").val(result.pomMaking);
                mpsaWindow.wrapper.find("#pom-packing").val(result.pomPacking);
                mpsaWindow.wrapper.find("#pom-planing").val(result.pomPlaning);
                mpsaWindow.wrapper.find("#pom-Making-Remark").val(result.pomMakingRemark);
                mpsaWindow.wrapper.find("#pom-Packing-Remark").val(result.pomPackingRemark);
                mpsaWindow.wrapper.find("#pom-Planing-Remark").val(result.pomPlaningRemark);
                mpsaWindow.wrapper.find("#pom-making").kendoNumericTextBox({ format: "0" });
                mpsaWindow.wrapper.find("#pom-packing").kendoNumericTextBox({ format: "0" });
                mpsaWindow.wrapper.find("#pom-planing").kendoNumericTextBox({ format: "0" });
            }
        });
        var columnTitle = $("#grid table tr:nth-child(1) th:nth-child(" + (ColIndexEditing + 1) + ")").text();
        mpsaWindow.title("Update MPSA_" + columnTitle);
        mpsaWindow.open().center();
    }
    if (element.hasClass("pr-cell-popup")) {
        ColIndexEditing = $(element.parent()).parent().children().index($(element.parent()));
        RowIndexEditing = $(element.parent()).parent().parent().children().index($(element.parent()).parent());
        var measureId = $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(3)').text();
        var lineId = element.parent().attr('LineId');
        prWindow.wrapper.find("#measureId").val(measureId);
        prWindow.wrapper.find("#lineId").val(lineId);
        $.ajax({
            cache: false,
            url: "/DdsMeeting/PrUpdate?date=" + dateSearch + "&measureId=" + measureId + "&lineId=" + lineId,
            type: 'Get',
            complete: function (data) {
                var result = jQuery.parseJSON(data.responseText);
                prWindow.wrapper.find("#pr-lastday").val(result.prLastDay);
                prWindow.wrapper.find("#pr-mtd").val(result.prMtd);
                prWindow.wrapper.find("#pr-lastday-remark").val(result.prLastDayRemark);
                prWindow.wrapper.find("#pr-mtd-remark").val(result.prMtdRemark);
                prWindow.wrapper.find("#pr-lastday").kendoNumericTextBox();
                prWindow.wrapper.find("#pr-mtd").kendoNumericTextBox();
            }
        });
        var columnTitle = $("#grid table tr:nth-child(1) th:nth-child(" + (ColIndexEditing + 1) + ")").text();
        prWindow.title("Update PR_" + columnTitle);
        prWindow.open().center();
    }
};

function onChange(oParam) {
    "use strict";
    var that, sThisDay = "";
    var iRowIndex = -1, iColIndex = -1;
    var selected = $.map(this.select(), function (item) {
        var arrElements = $(item).parent().children();
        var iCounter = 0, iLength = 0
        var arrParentElements = $(item).parent().parent().children();
        var oCurrentParent = $(item).parent();
        var iPageOffset = 0;
        if (oParam && oParam.sender && oParam.sender.pager && oParam.sender.pager.dataSource && oParam.sender.pager.dataSource._page && oParam.sender.pager.dataSource._pageSize) {
            iPageOffset = ((oParam.sender.pager.dataSource._page - 1) * oParam.sender.pager.dataSource._pageSize);
        }
        iRowIndex = $.inArray(oCurrentParent[0], arrParentElements) + iPageOffset;
        iLength = arrElements.length;
        for (iCounter = 0; iCounter < iLength; iCounter++) {
            if (arrElements[iCounter] === item) {
                iColIndex = iCounter;
                //alert("Row Number: " + iRowIndex + ", Col Number: " + iColIndex);
                break;
            }
        }
    });

    //get LineId from attribute of cell selected
    var lineId = $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').attr('LineId');
    var result = $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').children().val();
    $.ajax({
        url: "/DdsMeeting/ResultUpdate",
        data: {
            lineId: lineId,
            measureId: oParam.model.MeasureId,
            date: dateSearch,
            result: result
        },
        type: "POST",
        dataType: 'application/json',
        complete: function (data) {
            var jData = jQuery.parseJSON(data.responseText);
            if (jData.updated) {
                var row = jQuery.grep(measures, function (m) { return m.Id == jData.measureId; })[0].Index;
                var col = jQuery.grep(lines, function (l) { return l.Id == jData.lineId; })[0].Index;
                var dataSource = $("#grid").data("kendoGrid").dataSource;
                dataSource.data()[row].Lines[col].set("Value", jData.result);
                var updatecell = '#grid table tbody > tr:nth-child(' + (row + 1) + ') > td:nth-child(' + (col + 8) + ')';
                setColorForCellWhenEditSuccess(row, col + 7, jData.result);
                if ($(updatecell).find('input').val() == undefined) {
                    $(updatecell).text(jData.result);
                }
                else
                    $(updatecell).find('input').val(jData.result);

            }


            //set color for cell when edit success
            if (jQuery.parseJSON(data.responseText).status == "success") {
                setColorForCellWhenEditSuccess(iRowIndex, iColIndex, result);
            }
        }
    });
    return false;
};

function setColorForCellWhenEditSuccess(iRowIndex, iColIndex, result) {
    //if ($('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').attr('is-editable') == 'false')
    //    return;

    //get condition from target at column 6th
    var target = $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(6)').text();
    if (
        (
            ($('#grid tbody tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(3)').text() == 33 && result != 0)
            || ($('#grid tbody tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(3)').text() != 33)
        )//(check measure is 33 and result != 0) or (measure != 33)
        &&
        (
            (target.indexOf(">") >= 0 || target.indexOf("<") >= 0 || target.indexOf("=") >= 0)//check if have Operators
                && (!isNaN(parseFloat(result)) && isFinite(result))// check if it is number and not null
        )
      ) {
        var clause = "parseFloat('" + result + "')" + target;
        var asdfasdf = eval(clause);
        //console.log(clause , asdfasdf);
        if (eval(clause)) {
            $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').css('background-color', 'green');
            $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').css('color', 'white');
        }
        else {
            $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').css('background-color', 'red');
            $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').css('color', 'white');
        }
    }
    else {
        $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').css('background-color', 'white');
        $('#grid table tr:nth-child(' + (iRowIndex + 1) + ') td:nth-child(' + (iColIndex + 1) + ')').css('color', '#787878');
    }
};


var headerColumnRemakWindow = $("<div id='window'/>").kendoWindow({
    title: "Line Remark",
    visible: false,
    width: "400px",
    modal: true,
    viewable: false,
    content: {
        template: $("#windowTemplateHeaderColumnRemarkForMaking").html()
    },

    //close: onClose,
    //open: onOpen,

}).data("kendoWindow");

$("#yesBtnColumnRemarkConfirm").click(function () {
    $.ajax({
        cache: false,
        url: "/DdsMeeting/LineRemarkUpdate?date=" + dateSearch,
        data: $("#lineremark-update").serialize(),
        type: 'post',
        complete: function (data) {
            $('#grid table thead tr:nth-child(1) th:nth-child(' + ColIndexEditing + ')').attr('title', jQuery.parseJSON(data.responseText).result);
        }
    });
    //prWindow.close();
    //$.ajax({
    //    url: "/DdsMeeting/LineRemarkUpdate",
    //    data: {
    //        lineId: LineId,
    //        measureId: MeasureId,
    //        date: dateSearch,
    //        remark: $('#cellRemarkResult').val()
    //    },
    //    type: "POST",
    //    dataType: 'application/json',
    //    complete: function (data) {
    //        $('#grid tr:nth-child(' + (RowIndexEditing + 1) + ') td:nth-child(' + (ColIndexEditing + 1) + ')').children('.cell-remark').attr('title', jQuery.parseJSON(data.responseText).remark);
    //    }
    //});
    headerColumnRemakWindow.close();
});
$("#noBtnColumnRemarkConfirm").click(function () {
    headerColumnRemakWindow.close();
});

$("#grid").on("dblclick", 'th[data-field$=".Value"]', function (e) {
    var colIndex = $('#grid table thead tr th').index($(e.currentTarget));
    ColIndexEditing = colIndex + 1;
    var lineId = $(e.currentTarget).attr('LineId');
    headerColumnRemakWindow.wrapper.find("#departmentId-LineRemark").val(departmentId);
    headerColumnRemakWindow.wrapper.find("#lineId-LineRemark").val(lineId);
    $.ajax({
        cache: false,
        url: "/DdsMeeting/LineRemarkUpdate?date=" + dateSearch + "&departmentId=" + departmentId + "&lineId=" + lineId,
        type: 'Get',
        complete: function (data) {
            var result = jQuery.parseJSON(data.responseText);
            headerColumnRemakWindow.wrapper.find("#columnRemarkResult").val(result.LineRemark);
        }
    });
    headerColumnRemakWindow.title("Line Remark: " + $(e.currentTarget).text());
    headerColumnRemakWindow.open().center();
});

//$('#grid').bind('mousewheel DOMMouseScroll', function (e) {
//    var scrollTo = null;

//    if (e.type == 'mousewheel') {
//        scrollTo = (e.originalEvent.wheelDelta * -1);
//    }
//    else if (e.type == 'DOMMouseScroll') {
//        scrollTo = 40 * e.originalEvent.detail;
//    }

//    if (scrollTo) {
//        e.preventDefault();
//        $("#grid .k-grid-content").scrollTop(scrollTo + $("#grid .k-grid-content").scrollTop());
//    }
//    console.log(scrollTo);
//});


var tableOffset = $("#grid").offset().top;



//Using scroll in table need comment
//$(window).bind("scroll", function () {
//    myFunc();
//});

//$(window).bind("resize", function () {
//    resizeFixed();
//});

//function myFunc() {
//    var offset = $(this).scrollTop();

//    if (offset >= tableOffset) {
//        $("#grid thead").addClass("fixed");
//        $('.k-grid-content').css('margin-top', '42px');
//        resizeFixed()

//    }
//    else if (offset < tableOffset) {
//        $("#grid thead").removeClass("fixed");
//        $('.k-grid-content').css('margin-top', '0');
//        resizeFixed()
//    }
//}

//function resizeFixed() {
//    $("#grid thead").find("th").each(function (index) {
//        $(this).outerWidth($("#grid tbody tr:nth-child(1)").find("td").eq(index).outerWidth());
//    });
//}