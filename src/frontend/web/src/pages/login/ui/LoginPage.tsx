import { type ActionFunctionArgs, redirect, useActionData, useNavigation, useNavigate } from "react-router-dom";
import { loginWithSRP } from "../api/login";
import { LoginForm } from "@/widgets/login-form/ui/LoginForm.tsx";
import { useEffect } from "react";
import { refreshAuth } from "@/entities/session/model/authStore";
import { useAuth } from "@/entities/session/model/useAuth";

export async function loginAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();
    const identifier = formData.get("identifier") as string;
    const password = formData.get("password") as string;

    if (!identifier || !password) {
        return { error: { errors: { body: ["Username or email and password are required"] } } };
    }

    try {
        const authData = await loginWithSRP(identifier, password);

        void refreshAuth();

        return redirect(`/folder/${authData.rootFolderId}`);
    } catch (error: any) {
        return { error: { errors: { body: [error.message || "Failed to sign in. Please verify your credentials."] } } };
    }
}

export function LoginPage() {
    const actionData = useActionData<typeof loginAction>();
    const navigation = useNavigation();
    const navigate = useNavigate();
    const isSubmitting = navigation.state === "submitting";
    const auth = useAuth();

    const errorMessage = actionData?.error?.errors?.body?.[0];

    useEffect(() => {
        document.title = "Login - noKeyCloud";
    }, []);

    useEffect(() => {
        if (auth.status === "authenticated") {
            const target = auth.rootFolderId ? `/folder/${auth.rootFolderId}` : "/";
            navigate(target, { replace: true });
        }
    }, [auth.status, auth.rootFolderId, navigate]);

    return (
        <div className="flex flex-1 w-full items-center justify-center p-6 md:p-10">
            <div className="w-full max-w-sm">
                <LoginForm error={errorMessage} isSubmitting={isSubmitting} />
            </div>
        </div>
    );
}