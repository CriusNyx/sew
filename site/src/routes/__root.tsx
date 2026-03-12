import { createRootRoute, Link, Outlet } from "@tanstack/react-router";
import SewIcon from "../graphics/SewIcon.png";
import { useScrollIsLocked } from "../store/scroll.ts";
import { useNavigationIsHidden } from "../store/navigation.ts";

const RootLayout = () => {
  const isScrollLocked = useScrollIsLocked();
  const navigationIsHidden = useNavigationIsHidden();

  return (
    <div
      className={`flex flex-col ${
        isScrollLocked ? "h-screen w-screen overflow-hidden" : ""
      }`}
    >
      <div className="flex flex-col self-center items-center w-300 max-w-[95vw] p-5">
        <Link className="sew" to="/">
          <h1 className="flex relative flex-row items-center gap-5 pointer-none no-underline! mb-0! title">
            <img className="w-20 h-20" src={SewIcon} />
            Sew
          </h1>
        </Link>
        {!navigationIsHidden && (
          <div className="flex flex-row gap-5 pt-2">
            <Link to={"/"}>Home</Link>
            <a href={"https://github.com/CriusNyx/sew/"}>Github</a>
            <Link to={"/blog"}>Blog</Link>
            <Link to={"/docs"}>Docs</Link>
          </div>
        )}
      </div>
      <div
        className={`flex flex-col self-center items-start px-5 grow h-full ${isScrollLocked ? "" : "overflow-hidden"}`}
      >
        <div className="flex flex-col rounded-xl w-300 max-w-[95vw] bg-black/50 p-5 px-10 mb-5 h-full">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export const Route = createRootRoute({ component: RootLayout });
