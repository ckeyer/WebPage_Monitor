关于html解析的说明
==================

### 状态参数(Status.Key)

> 0 --- TAG_STARTS
>
> 1 --- NAME
>
> 2 --- WHITESPACE
>
> 3 --- ATTR
>
> 4 --- ASSIGN
>
> 5 --- QUOTED_VALUE
>
> 6 --- TAG_ENDS
>
> 7 --- TEXT
>
> 8 --- ATOM
>
> 9 --- CLOSING
>
> 10 --- COMMENT_STARTS
>
> 11 --- COMMENT_BODY
>
> 12 --- COMMENT_ENDS
>


### 状态参数(operate)
> 0 --- 节点将要创建，节点完毕
>
> 1 --- 节点创建中
>
> 2 --- 节点操作中
>
> 4 --- 节点结束中
>
> 8 --- 
>