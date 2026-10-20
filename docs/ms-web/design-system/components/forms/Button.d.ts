export interface ButtonProps{
children:React.ReactNode;
variant?:'primary'|'secondary'|'subtle'|'danger';
size?:'small'|'medium'|'large';
disabled?:boolean;
icon?:string;
onClick?:()=>void;
}
