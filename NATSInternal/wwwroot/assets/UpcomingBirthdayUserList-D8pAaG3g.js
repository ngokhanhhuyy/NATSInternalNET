import{d as g,L as w,P as x,j as v,ag as k,z as L,a as t,c as o,b as s,A as l,D as p,u as d,F as U,f as B,t as u,e as N,T as S,s as I,x as A,_ as C}from"./index-t81Ut7qy.js";import{u as D}from"./viewStatesComposable-B_gfPsac.js";const h=a=>(I("data-v-5116f639"),a=a(),A(),a),T={class:"block block-upcoming-birthday-users bg-white p-0 h-100 rounded-3"},V=h(()=>s("span",{class:"text-primary small fw-bold"}," SINH NHẬT THÁNG NÀY ",-1)),j=h(()=>s("i",{class:"bi bi-arrow-counterclockwise"},null,-1)),F=[j],H={class:"block-body bg-white border border-top-0 rounded-bottom"},P={key:0},R={key:0,class:"list-group list-group-flush top-effective-users-list"},z=["src"],M={class:"d-flex flex-column flex-fill name"},E={class:"fw-bold"},G={class:"small opacity-50"},K={class:"badge bg-success-subtle border border-success-subtle text-success rounded small ms-2 handled-orders fw-bold"},Y={key:1,class:"d-flex align-items-center justify-content-center p-3 opacity-50"},q=g({__name:"UpcomingBirthdayUserList",async setup(a){let n,m;const _=w(),i=([n,m]=x(()=>b()),n=await n,m(),n),{loadingState:r}=D();async function b(){const c=await _.getUpcomingBirthdayUsersAsync();return v(new k(c))}async function y(){if(!r.isLoading){r.isLoading=!0;const c=await _.getUpcomingBirthdayUsersAsync();i.mapFromResponseDto(c),r.isLoading=!1}}return(c,J)=>{const f=L("RouterLink");return t(),o("div",T,[s("div",{class:"block-header bg-primary bg-opacity-25 border border-primary-subtle rounded-top ps-3 p-2 d-flex justify-content-between align-items-center"},[V,s("button",{class:"btn btn-primary btn-sm",onClick:y},F)]),s("div",H,[l(S,{name:"fade",mode:"out-in"},{default:p(()=>[d(r).isLoading?N("",!0):(t(),o("div",P,[d(i).results!=null&&d(i).results.length?(t(),o("ul",R,[(t(!0),o(U,null,B(d(i).results,e=>(t(),o("li",{class:"list-group-item d-flex flex-row align-items-center px-3 py-2",key:e.id},[l(f,{to:{name:"userProfile",params:{userId:e.id}}},{default:p(()=>[s("img",{src:e.avatarUrl,class:"rounded-circle me-2"},null,8,z)]),_:2},1032,["to"]),s("div",M,[l(f,{to:{name:"userProfile",params:{userId:e.id}}},{default:p(()=>[s("span",E,u(e.fullName),1)]),_:2},1032,["to"]),s("span",G,"@"+u(e.userName),1)]),s("span",K,u(e.daysLeftToBirthday),1)]))),128))])):(t(),o("div",Y," Không có nhân viên có sinh nhật sắp tới "))]))]),_:1})])])}}}),W=C(q,[["__scopeId","data-v-5116f639"]]);export{W as default};
