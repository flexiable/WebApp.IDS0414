/*@author Devilu*/

base_num = $(".ul_num li").eq(3);
out_width = base_num.children().css("width").substring(0, base_num.children().css("width").length -2 )
inner_width = base_num.children().children().css("width").substring(0, base_num.children().children().css("width").length -2 )
need_add = (out_width-inner_width)/2

base_line= $(".ul_num li").eq(1);
line_width = base_line.children().css("width").substring(0, base_line.children().css("width").length -2 )

new_line_width = Number(line_width)+Number(need_add)

/** 设置字体居中	**/
var ul_li_p_list = $(".ul_word li p")
ul_li_p_list.each(function(){
	var width = $(this).css("width").substring(0, ($(this).css("width").length - 2) )
	$(this).css({
		"margin-left":-(width/4) + "px"
	})
})

function follow_show(){
	var last_blue_num = 0
	var ul_num_list = $(".ul_num li div");
	ul_num_list.each(function(index){
		var c = $(this).attr("class")
		if(c == "num_blue" && index > last_blue_num){
			last_blue_num = index
		}
	})
	var follow_show_list = $(".follow_show ul li")
	if(last_blue_num == 0){
		follow_show_list.eq(last_blue_num).show()
	}else{
		pos = last_blue_num  / 3
		follow_show_list.eq(pos-1).hide()
		follow_show_list.eq(pos).show()
	}
	
}

follow_show()

function next(current){

	parent_li = $(current).parents("li")
	var index = $(".follow_show ul li").index(parent_li);
	pos = 3 * (index + 1)
	last_num = $(".ul_num li").eq(pos)
	last_num.prev().children().removeClass("line_gray").addClass("line_blue")
	
	last_num.prev().children().css({
		"width":new_line_width+"px"
	})
	last_num.children("div").remove();
	last_num.html(
		"<div class='num_blue'>"+(index+2)+"</div>"
		);
	last_num.next().children().removeClass("line_gray").addClass("line_blue")
	last_num.next().children().css({
		"width":new_line_width+"px"
	})
	follow_show()
}