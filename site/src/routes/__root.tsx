import { createRootRoute, Link, Outlet } from "@tanstack/react-router";
import { useScrollIsLocked } from "../services/ScrollControlService.tsx";

const RootLayout = () => {
  const isScrollLocked = useScrollIsLocked();

  return (
    <div
      className={`flex flex-col ${
        isScrollLocked ? "h-screen w-screen overflow-hidden" : ""
      }`}
    >
      <div className="flex flex-col self-center items-center w-300 max-w-[95vw] p-5">
        <Link className="sew" to="/">
          <h1 className="flex relative flex-row items-center gap-5 pointer-none no-underline! mb-0! title">
            <img className="w-20 h-20" src="SewIcon.png" />Sew
          </h1>
        </Link>
        <div className="flex flex-row gap-5 self-start">
          <Link to={"/"}>Home</Link>
          <Link to={"/blog"}>Blog</Link>
          <Link to={"/docs"}>Docs</Link>
        </div>
      </div>
      <div className="flex flex-col self-center items-start overflow-scroll px-5 grow h-full">
        <div className="flex flex-col rounded-xl w-300 max-w-[95vw] bg-black/50 p-5 px-10 mb-5 h-full">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export const Route = createRootRoute({ component: RootLayout });
